using Chroma.Input;
using System;
using System.Collections.Generic;
using Chroma.Input.EventArgs;

namespace Commodore.Framework.Managers
{
    public class DebugManager
    {
        private Dictionary<KeyCode, Action> DebugActions { get; }

        public DebugManager()
        {
            DebugActions = new Dictionary<KeyCode, Action>();
        }

        internal void KeyPressed(KeyEventArgs e)
        {
            if (DebugActions.ContainsKey(e.KeyCode))
            {
                DebugActions[e.KeyCode]?.Invoke();
            }
        }

        public void BindDebugAction(KeyCode key, Action action)
        {
            if (DebugActions.ContainsKey(key))
                return;

            DebugActions.Add(key, action);
        }
    }
}
