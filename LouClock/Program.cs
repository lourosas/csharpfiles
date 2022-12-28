using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LouClock
{
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
         c = new Clock();
         //lt = new LTimer();
         //c.AddObserver(lt);
         //lt.Start();

      }
   }

   class Clock
   {
      private long time = 0;
      private AutoResetEvent autoEvent;
      private Timer timer;
      private DateTime begin;
      private DateTime current;
      private ClockNotifier clockNotifier;

      public Clock()
      {
         this.begin     = DateTime.Now;
         this.autoEvent = new AutoResetEvent(false);
         this.timer     = new Timer(SetTime,autoEvent,0,100);
         this.clockNotifier = new ClockNotifier();
         //this.timer     = new Timer(SetTime, null, 0, 100);
         //this.clockNotifier = new ClockNotifier();
         this.autoEvent.WaitOne();
         this.timer.Dispose();
      }

      public void AddObserver(IClockObserver observer) 
      {
         Console.WriteLine(observer);
         this.clockNotifier.AddObserver(observer); 
      }

      private void SetTime(Object? stateInfo)
      {
         if (stateInfo != null)
         {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
         }
         this.current = DateTime.Now;
         //this.clockNotifier.Trigger();
         //this.clockNotifier.SetTime(this.current);
         //most of this needs to eventually be removed
         if (this.time == 10)
         {
            /*
            Console.Write("Current Time:  ");
            Console.WriteLine(this.current.ToString("HH:mm:ss.fff"));
            Console.WriteLine(this.current.ToString());
            Console.WriteLine("{0:N0}", (this.current.Ticks - this.begin.Ticks));
            this.begin = this.current;
            */
            this.clockNotifier.Trigger();
            this.clockNotifier.SetTime(this.current);
            time = 0;
         }
         ++time;
      }
   }

   class ClockNotifier
   {
      private DateTime current;
      private Timer timer;
      private Boolean trigger;
      private List<IClockObserver> observers;

      public ClockNotifier()
      {
         const int SLEEP = 10;
         this.trigger = false;
         this.observers = new List<IClockObserver>();
         this.timer = new Timer(SetTime, null, 0, SLEEP);
      }

      public void AddObserver(IClockObserver observer)
      {
         Console.WriteLine("Add the fucking observer");
         this.observers.Add(observer);
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
            Console.WriteLine(this.current.ToString("HH:mm:ss.fff"));
            Console.WriteLine(this.current.Ticks);
            this.trigger = false;
         }
      }
   }


   public interface IClockObserver
   {
      public void updateTime(DateTime time);
      public void updateTime(long ticks);
   }

   class LTimer : IClockObserver
   {
      private Boolean run = false;
      private Boolean receive = false;
      private DateTime begin;
      private DateTime current;
      private TimeSpan elapsed;
      private long ticks = 0;
      private long milliseconds = 0;
      private long seconds = 0;
      private long minutes = 0;
      private long hours = 0;
      private long days = 0;

      /////////////////////////////Constructor(s)/////////////////////
      public LTimer() { }

      /////////////////////////////Public Methods/////////////////////
      public void Reset() { }

      public void Start() 
      {
         this.SetRun(true);
         this.SetReceive(true);
      }

      public void Stop() { }
      //////////////////////////Protected Methods/////////////////////
      ////////////////////////////Private Methods/////////////////////
      private void CalculateTime() { }
      private void ClearAllTimeValues() { }
      private void NotifyObservers() { }
      private void SetRun(Boolean toRun)
      {
         this.run = toRun;
         Console.WriteLine(this.run);
      }

      private void SetReceive(Boolean toReceive) 
      {
         this.receive = toReceive;
         Console.WriteLine(this.receive);
      }

      private void SetTime(DateTime time) { }

      private void SetTimeValues() { }

      ///////////////////////////Interface Methods////////////////////
      public void updateTime(DateTime time) { }
      public void updateTime(long ticks) { }
   }
}