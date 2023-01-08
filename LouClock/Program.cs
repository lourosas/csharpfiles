using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace LouClock
{
   public enum State 
   {
      Run,
      Stop
   }

   class Program
   {
      private LTimer lt;
      private Clock c;
      static void Main(string[] args)
      {
         new Program();
      }

      public Program()
      {
         
         Console.WriteLine("Hello World");
         //ThreadTester t = new ThreadTester();
         //Thread.Sleep(10000);
         //t.Release();
         lt = new LTimer();
         lt.Start();
         Thread.Sleep(30000);
         lt.Stop();
         Thread.Sleep(15000);
         lt.Start();
         //let it run forever so to speak...
         while (true) 
         {
            lt.Lap();
            Thread.Sleep(600000); 
         }
         //Thread.Sleep(35000);
         //lt.Quit();
         //c = new Clock();
         //lt = new LTimer();
         //c.AddObserver(lt);
         //lt.Start();

      }
   }

   class Clock
   {
      private long time = 0;
      private System.Threading.Timer timer;
      private DateTime begin;
      private DateTime current;
      private ClockNotifier clockNotifier;
      private Thread t;
      private Stopwatch stopwatch;

      public Clock()
      {
         this.stopwatch= new Stopwatch();
         this.clockNotifier = new ClockNotifier();
         t = new Thread(TestThread);
         t.Start();
         this.begin = DateTime.Now;
         Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
         this.stopwatch.Start();
         timer = new System.Threading.Timer(UpdateTime,null,0,1000);
         t.Join();
         this.stopwatch.Stop();
         Console.WriteLine("2:  " + this.stopwatch.ElapsedMilliseconds);
         Console.ReadLine();
         timer.Dispose();
         this.clockNotifier.setRun(false);
      }

      public void AddObserver(IClockObserver observer) 
      {
         Console.WriteLine(observer);
         this.clockNotifier.AddObserver(observer); 
      }

      private void TestThread()
      {
         int i = 0;
         while (i++ < 10) {
            Console.Write("WTF: ");
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("1a:  "+stopwatch.ElapsedMilliseconds);
            Thread.Sleep(1000);
         }
      }

      private void UpdateTime(Object? stateInfo)
      { 
         Console.WriteLine("1b:  "+this.stopwatch.ElapsedMilliseconds);
         if(stateInfo == null)
         {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            this.current = DateTime.Now;
            this.clockNotifier.Trigger();
            this.clockNotifier.SetTime(this.current);
         }
      }
   }

   class ClockNotifier
   {
      private DateTime current;
      private System.Threading.Timer timer;
      private Boolean trigger;
      private Boolean toRun;
      private List<IClockObserver> observers;

      public ClockNotifier()
      {
         const int SLEEP = 10;
         this.trigger = false;
         this.toRun = true;
         this.observers = new List<IClockObserver>();
         Thread t = new Thread(TestTime);
         t.Start();
         //this.timer = new System.Threading.Timer(SetTime, null, 0, SLEEP);
      }

      public void AddObserver(IClockObserver observer)
      {
         this.observers.Add(observer);
      }

      public void setRun(Boolean run)
      {
         this.toRun = run;
      }

      public void Trigger() 
      {
         this.trigger = true;
      }

      public void SetTime(DateTime datetime)
      {
         if (this.trigger)
         {
            this.current = datetime;
         }
      }

      private void SetTime(Object? stateInfo)
      {
         if(this.trigger && stateInfo == null)
         {
            Console.WriteLine("poop "+Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(this.current.ToString("HH:mm:ss.fff"));
            Console.WriteLine(this.current.Ticks);
            this.trigger = false;
         }
      }

      private void TestTime()
      {
         while (toRun) 
         {
            if(this.trigger)
            {
               Console.WriteLine("poop " + Thread.CurrentThread.ManagedThreadId);
               Console.WriteLine(this.current.ToString("HH:mm:ss.fff"));
               Console.WriteLine(this.current.Ticks);
               this.trigger = false;
            }
            Thread.Sleep(10);
         }
      }
   }


   public interface IClockObserver
   {
      public void setLaps(Stack<TimeSpan> laps);
      public void setState(State state);
      public void updateTime(DateTime time);
      public void updateTime(TimeSpan timespan);
      public void updateTime(long millis);
   }

   class LTimer
   {
      private AutoResetEvent autoResetEvent;
      private Boolean run = false;
      private Boolean receive = false;
      private Boolean quit = false;
      private Thread thread1;
      private Stopwatch stopwatch;
      private System.Threading.Timer timer;
      private IClockObserver observer;
      private TimeSpan current;
      private Stack<TimeSpan> lapStack;

      /////////////////////////////Constructor(s)/////////////////////
      public LTimer() 
      {
         this.stopwatch = new Stopwatch();
         this.observer = new SimpleTimerObserver();
         this.lapStack = new Stack<TimeSpan>();
         //Initialize the Current TimeSpan to Zero
         this.current = TimeSpan.Zero;
         SetRun(false);
         thread1 = new Thread(MonitorTheQuit);
         thread1.Start();
         this.SetUpTimer();
      }

      /////////////////////////////Public Methods/////////////////////
      public void Quit()
      {  
         this.quit = true;
      }

      public void Lap() 
      {
         TimeSpan toSave = this.stopwatch.Elapsed;
         TimeSpan lap = toSave.Subtract(current);
         this.current = toSave;
         //Console.WriteLine("Lap:  "+lap);
         //Store into a Laps<TimeSpan> Stack
         //alert the <<Observers>>
         //observer.Lap(Laps);
         this.lapStack.Push(lap);
         this.observer.setLaps(this.lapStack);

      }

      public void Reset() { }

      public void Start() 
      {
         this.SetRun(true);
         this.SetReceive(true);
         this.stopwatch.Start();
      }

      public void Stop()
      {
         this.SetRun(false);
         this.SetReceive(false);
         this.stopwatch.Stop();
         this.observer.updateTime(this.stopwatch.Elapsed);
      }
      //////////////////////////Protected Methods/////////////////////
      ////////////////////////////Private Methods/////////////////////
      private void CalculateTime() { }
      private void ClearAllTimeValues() { }
      private void ElapseTheTime(Object? stateInfo)
      {
         if (stateInfo != null)
         {
            AutoResetEvent fuckingResetEvent = (AutoResetEvent)stateInfo;
            if (this.run)
            {
               this.observer.updateTime(this.stopwatch.Elapsed);
            }
         }
      }

      private void MonitorTheQuit()
      {
         while (!quit) 
         {
            Thread.Sleep(1);
         }
         this.SetRun(false);
         this.SetReceive(false);
         //autoResetEvent.Set();
         autoResetEvent.Dispose();
         timer.Dispose();
         System.Environment.Exit(0);
      }

      /*
       * Going to eventually want to use this...
       */
      private void NotifyObservers()
      {
      }

      private void SetRun(Boolean toRun)
      {
         this.run = toRun;
         if (this.run)
         {
            this.observer.setState(State.Run);
         }
         else
         {
            this.observer.setState(State.Stop);
         }
      }

      private void SetReceive(Boolean toReceive) 
      {
         this.receive = toReceive;
      }

      private void SetTime(DateTime time) { }

      private void SetTimeValues() { }

      private void SetUpTimer() 
      {
         this.autoResetEvent = new AutoResetEvent(false);
         //This will definitely need to be changed...
         //timer = new System.Threading.Timer(ElapseTheTime, autoResetEvent, 0, 1000);
         //keep at 1/100 sec resolution for the time being...
         timer = new System.Threading.Timer(ElapseTheTime, autoResetEvent, 0, 10);
      }
   }

   class SimpleTimerObserver : IClockObserver 
   {
      private State currentState;
      private TimeSpan currentSpan;
      private readonly object timerlock = new object();
      public SimpleTimerObserver() 
      {
         this.currentState = State.Stop;
         this.currentSpan = new TimeSpan(-1);
      }

      //////////////////////Interface Implementations/////////////////
      public void setLaps(Stack<TimeSpan> laps)
      {
         Console.WriteLine("Laps:  ");
         foreach(TimeSpan span in laps) 
         {
            Console.WriteLine(span);
         }
         Console.WriteLine();
      }
      public void setState(State state)
      {
         this.currentState = state;
      }
      public void updateTime(DateTime time) { }

      public void updateTime(TimeSpan timespan)
      {
         //To avoid a race condition...
         lock (timerlock)
         {
            if ((currentState==State.Run && 
                (timespan.Seconds != this.currentSpan.Seconds))
                || currentState == State.Stop)
            {
               Console.WriteLine(timespan);
               //This is the prototype...
               Console.WriteLine("{0:0} {1:00}:{2:00}:{3:00}.{4:00}",
                  timespan.Days,timespan.Hours,timespan.Minutes,
                  timespan.Seconds,timespan.Milliseconds / 10);
               this.currentSpan = timespan;
            }
         }
      }

      public void updateTime(long millis)
      {
         Console.WriteLine("millis: " + millis);
      }
   }

   class ThreadTester
   {
      AutoResetEvent signal;
      private Thread thread0;
      private Boolean toRelease;
      private Thread thread1;

      public ThreadTester()
      {
         toRelease = false;
         this.signal = new AutoResetEvent(false);
         thread0 = new Thread(TestThread0);
         thread0.Start();
         //thread1 = new Thread(MonitorToRelease);
         thread1 = new Thread(SetUpTheRelease);
         thread1.Start();
         //this.SetUpTheRelease();
         //thread1.Join();
         //thread0.Join();
      }

      public void Release() 
      {
         this.toRelease = true;
      }

      private void MonitorToRelease() 
      {
         Thread.Sleep(50000);
         toRelease = true;
      }

      private void SetUpTheRelease()
      {
         while (!toRelease)
         {
            Thread.Sleep(1000);
            this.signal.Set();
         }

         Console.WriteLine("Fucking toRelease: " + toRelease);
      }

      private void TestThread0()
      {
         //for (int i = 0; i < 5; ++i)
         int i = 0;
         while(!toRelease)
         {
            Console.WriteLine("Waiting for signal..." + i);
            //Thread.Sleep(500);
            this.signal.WaitOne();
            ++i;
         }
         
         this.signal.Dispose();
         Console.WriteLine("Got the fucking signal!");
      }
   }
}