using Game.States;


namespace Game {
    public class Program {
        static void Main() {
            StateManager stateMgr = new StateManager();
            if (stateMgr.Setup())
                stateMgr.Go();
        }
    }
}