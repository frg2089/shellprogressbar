﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class CJKDeeplyNestedProgressBarTreeExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var random = new Random();

			var numberOfSteps = 7;

			var overProgressOptions = new ProgressBarOptions
			{
				DenseProgressBar = true,
				ProgressCharacter = '─',
				BackgroundColor = ConsoleColor.DarkGray,
				EnableTaskBarProgress = RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
			};

			using (var pbar = new ProgressBar(numberOfSteps, "总体进展", overProgressOptions))
			{
				var stepBarOptions = new ProgressBarOptions
				{
					DenseProgressBar = true,
					ForegroundColor = ConsoleColor.Cyan,
					ForegroundColorDone = ConsoleColor.DarkGreen,
					ProgressCharacter = '─',
					BackgroundColor = ConsoleColor.DarkGray,
					CollapseWhenFinished = true,
				};
				Parallel.For(0, numberOfSteps, (i) =>
				{
					var workBarOptions = new ProgressBarOptions
					{
						DenseProgressBar = true,
						ForegroundColor = ConsoleColor.Yellow,
						ProgressCharacter = '─',
						BackgroundColor = ConsoleColor.DarkGray,
					};
					var childSteps = random.Next(1, 5);
					using (var childProgress = pbar.Spawn(childSteps, $"步骤 {i} 进度", stepBarOptions))
						Parallel.For(0, childSteps, (ci) =>
						{
							var childTicks = random.Next(50, 250);
							using (var innerChildProgress = childProgress.Spawn(childTicks, $"步骤 {i}::{ci} 进度", workBarOptions))
							{
								for (var r = 0; r < childTicks; r++)
								{
									innerChildProgress.Tick();
									Program.BusyWait(50);
								}
							}
							childProgress.Tick();
						});

					pbar.Tick();
				});
			}
			return Task.FromResult(1);
		}
	}
}
