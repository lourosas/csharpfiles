using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
   class Program
   {
      /*Poop and more poop*/
      static void Main(string[] args)
      {
         Console.WriteLine(args.Length);
         Console.WriteLine("Hello World");
         var data = 1234;
         Console.WriteLine(data);
         new Program();
      }

      public Program()
      {
         Console.WriteLine("Starting Program");
         Console.WriteLine("================");
         this.SetUpA();
         this.SetUpB();
         Console.WriteLine("================");
         Console.WriteLine("The End");
      }

      private void SetUpA() {
         Console.WriteLine("SetUpA()");
         AClass a = new AClass();
         Console.WriteLine(a.ToString);
         AClass a1 = new AClass(12);
         Console.WriteLine(a1.ToString);
      }

      private void SetUpB() {
         Console.WriteLine("SetUpB()");
         BClass b0 = new BClass();
         BClass b1 = new BClass(150);
         BClass b2 = new BClass(174, 2);
         BClass b3 = new BClass(8, 101, 78);
         Console.WriteLine(b0.ToString);
         Console.WriteLine(b1.ToString);
         Console.WriteLine(b2.ToString);
         Console.WriteLine(b3.ToString);
      }
   }

   public class AClass
   {
      private int simple0 = 0;
      private int simple1;

      public AClass()
      {
         const int V = -1;
         this.simple1 = V;
      }

      public AClass(int value)
      {
         this.simple1 = value;
      }

      public int Simple0() { return simple0; }

      public int Simple1() { return simple1; }


      public new String ToString => this.simple0.ToString() + ", " + this.simple1.ToString();

   }

   public class BClass : AClass
   {
      private int bsimple  = 0;
      private int bsimple1 = 23;

      public BClass() {  }

      public BClass(int value)
         : base(value) //almost similar to the C++ syntax
      { }

      public BClass(int value0, int value1)
      : base(value0)
      {
         this.bsimple = value1;
      }

      public BClass(int value0, int value1, int value2)
      : base(value0)
      {
         this.bsimple  = value1;
         this.bsimple1 = value2;
      }

      public new String ToString => base.ToString + ", " +
         this.bsimple.ToString() + ", " + this.bsimple1.ToString();
   }
}
