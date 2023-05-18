using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using static System.Console;
using System.Xml.Linq;

namespace WebApplication1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build();

			var state = new StateObject();
			for (int i = 0; i < 5; i++)
			{
				Task.Run(() => new SampleTask().RaceCondition(state));
	
				Console.WriteLine(state);
			}
		}

		public class StateObject
		{
			private int _state = 5;
			 private object sync = new object();
			public void ChangeState(int  loop)
			{
				lock(sync)
				{

					if (_state == 5)
					{
						_state++;
						if (_state != 6)
						{
							Console.WriteLine($"Race conditon occurred after {loop} loops");
							Trace.Fail($"ดํมห{loop}");

						}
					}
				}
				
				_state = 5;
			}
			
		}

		public class SampleTask
		{
			public void RaceCondition(object o)
			{
				Trace.Assert(o is StateObject, "o must be of type StateObject");
				StateObject state = o as StateObject;
				int i = 0;
				while (true)
                {
					lock(state)
					{
						state.ChangeState(i++);

					}
				}
			}
		}
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
