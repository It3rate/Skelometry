using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slugs
{
    static class Program
    {
        private static int _formIndex = -1;
        private static Form ActiveForm;
        private static List<Form> Forms;

        [STAThread]
        static void Main()
        {
	        Application.EnableVisualStyles();
	        Application.SetCompatibleTextRenderingDefault(false);
	        Forms = new List<Form>
	        {
		        new SlugForm(),
		        //new VisForm(),
	        };

	        NextForm();

	        Application.Run(ActiveForm);
        }

        public static void NextForm()
        {
	        _formIndex++;
	        if (_formIndex >= Forms.Count)
	        {
		        _formIndex = 0;
	        }

	        foreach (var form in Forms)
	        {
		        form.StartPosition = FormStartPosition.CenterScreen;
		        if (form.Visible)
		        {
			        form.Hide();
		        }
	        }

	        ActiveForm = Forms[_formIndex];
	        ActiveForm.Show();
        }
    }
}
