using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class AskEvent : EventArgs
    {
        private Player asked;
        public Player Asked
        {
            get { return this.asked; }
            set { this.asked = value; }
        }

        private string keyname;
        public string KeyName
        {
            get { return this.keyname; }
            set { this.keyname = value; }
        }

        private string message;
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }
        
        private bool isyes;
        public bool IsYes
        {
            get { return this.isyes; }
            set { this.isyes = value; }
        }
    }

    public delegate void AskEventHandler(object sender, AskEvent e);
}
