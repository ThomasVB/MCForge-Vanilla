using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MCForge.Commands
{
    class CmdDebug : Command
    {
        public override string name
        {
            get { return "debug"; }
        }

        public override string shortcut
        {
            get { return String.Empty; }
        }

        public override string type
        {
            get { return "debug"; }
        }

        public override bool museumUsable
        {
            get { return true; }
        }

        public override LevelPermission defaultRank
        {
            get { return LevelPermission.Admin; }
        }

        public override void Use(Player p, string message)
        {
            var split = message.Split(' ');
            switch (split[0])
            {
                case "savechanges":
                    if(p != null)
                        p.level.saveChanges();
                    break;
                case "tcount":
                    Process pr = Process.GetCurrentProcess();
                    Player.SendMessage(p, "Thread count: " + pr.Threads.Count);
                    break;
            }
        }

        public override void Help(Player p)
        {
            //throw new NotImplementedException();
        }
    }
}
