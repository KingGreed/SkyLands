using System;
using Mogre;

using Game.States;
using Game.BaseApp;


namespace Game
{
    public class Program : StateManager
    {
        static void Main()
        {
            new Program().Go();
        }
    }
}