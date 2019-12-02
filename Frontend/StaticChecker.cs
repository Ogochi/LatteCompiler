using System.Linq;
using Frontend.StateManagement;
using ParsingTools;

namespace Frontend
{
    public class StaticChecker
    {
        public void CheckProgram(LatteParser.ProgramContext program)
        {
            program.topDef().ToList().ForEach(FrontendEnvironment.Instance.AddTopDef);
                
            program.EnterRule(new StaticCheckListener());
        }
    }
}