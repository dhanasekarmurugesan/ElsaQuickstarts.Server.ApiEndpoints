using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;

namespace ElsaQuickstarts.Server.ApiEndpoints.Activities
{
    [Activity(Category = "KZN File IO", DisplayName = "Write Contents to File", Description ="Write Default Text contents to a text file", Outcomes = new[] { OutcomeNames.Done })]
    public class WriteToFileActivity : Activity, IQCustomActivity
    {
        public WriteToFileActivity() 
        { 
        }

        protected override IActivityExecutionResult OnExecute()
        {
            if (Write())
            {
                return Done();
            }

            //Customize to return something to denote it failed.
            return Done();
        }

        private bool Write()
        {
            Guid guid = Guid.NewGuid();

            string basePath = @"C:\Elsa";
            string filePath = Path.Combine(basePath, guid.ToString()+ ".txt");

            try
            {
                File.WriteAllText(filePath, $"Workflow trigger at {DateTime.Now}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
