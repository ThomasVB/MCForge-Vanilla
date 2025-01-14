﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Updater
{
    class Program
    {
        static int tries = 0;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(globalException);
            if (args.Length < 1)
            {
                string[] defaultname = new string[] { "MCForge.exe" };
                Main(defaultname);
            }
            else
            {
                try
                {
                    Console.WriteLine("Waiting for " + args[0] + " to exit...");
                    while (Process.GetProcessesByName(args[0]).Length > 0)
                    {
                        //Sit here and do nothing
                    }
                }
                catch (Exception e) { UpdateFailure(e); }
                Update(args);
            }
        }
        static void Update(string[] args)
        {
            Console.WriteLine("Updating MCForge...");
            try
            {
                tries++;
                if (File.Exists("MCForge.update") || File.Exists("MCForge_.update"))
                {
                    try
                    {
                        if (File.Exists("MCForge.update"))
                        {
                            if (File.Exists(args[0]))
                            {
                                if (File.Exists("MCForge.backup"))
                                    File.Delete("MCForge.backup");
                                File.Move(args[0], "MCForge.backup");
                            }
                            File.Move("MCForge.update", args[0]);
                        }
                    }
                    catch (Exception e)
                    {
                        if (tries > 4)
                        {
                            UpdateFailure(e);
                        }
                        else
                        {
                            Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                            Thread.Sleep(100);
                            Update(args);
                        }
                    }
                    try
                    {
                        if (File.Exists("MCForge_.update"))
                        {
                            if (File.Exists("MCForge_.dll"))
                            {
                                if (File.Exists("MCForge_.backup"))
                                    File.Delete("MCForge_.backup");
                                File.Move("MCForge_.dll", "MCForge_.backup");
                            }
                            File.Move("MCForge_.update", "MCForge_.dll");
                        }
                    }
                    catch (Exception e)
                    {
                        if (tries > 4)
                        {
                            UpdateFailure(e);
                        }
                        else
                        {
                            Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                            Thread.Sleep(100);
                            Update(args);
                        }
                    }
                }
                else
                {
                    NoUpdateFiles();
                }
                Console.WriteLine("MCForge successfully updated.  Starting MCForge...");
                try
                {
                    Process.Start(args[0]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to start MCForge.  You will need to start it manually.");
                    MessageBox.Show("Updater has updated MCForge, but was unable to start it.  You will need to start it manually.", "Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                if (tries > 4)
                {
                    UpdateFailure(e);
                }
                else
                {
                    Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                    Thread.Sleep(100);
                    Update(args);
                }
            }
        }
        static void UpdateFailure(Exception e)
        {
            Console.WriteLine("Updater is unable to update MCForge.\n\n" + e.ToString() + "\n\nPress any key to exit.");
            MessageBox.Show("Updater is unable to update MCForge.\n\n" + e.ToString(), "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.ReadLine();
            Environment.Exit(0);
        }
        static void NoUpdateFiles()
        {
            Console.WriteLine("Updater has no files to update.  Press any key to exit.");
            MessageBox.Show("Updater has no files to update.", "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.ReadLine();
            Environment.Exit(0);
        }
        static void globalException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("UnhandledException:\n\n" + e);
            MessageBox.Show("UnhandledException:\n\n" + e, "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
