/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCForge
{
    public sealed class CommandList
    {
        public List<Command> commands = new List<Command>();
        public CommandList() { }
        public void Add(Command cmd) { commands.Add(cmd); }
        public void AddRange(List<Command> listCommands)
        {
            listCommands.ForEach(delegate(Command cmd) { commands.Add(cmd); });
        }
        public List<string> commandNames()
        {
            return commands.Select(c => c.name).ToList();
        }

        public bool Remove(Command cmd) { return commands.Remove(cmd); }
        public bool Contains(Command cmd) { return commands.Contains(cmd); }
        public bool Contains(string name)
        {
            string searchFor = name.ToLower();
            return commands.Any(c => c.name == searchFor);
        }
        public Command Find(string name)
        {
            string searchFor = name.ToLower();
            return commands.FirstOrDefault(c => c.name == searchFor || c.shortcut == searchFor);
        }

        public string FindShort(string shortcut)
        {
            if (String.IsNullOrEmpty(shortcut))
                return String.Empty;

            string searchFor = shortcut.ToLower();
            foreach (Command cmd in commands)
            {
                if (cmd.shortcut == searchFor)
                    return cmd.name;
            }

            return String.Empty;
        }

        public List<Command> All() { return new List<Command>(commands); }
    }
}