using OfficeOpenXml;
using OfficeOpenXml.Style;
using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace SchoolRing
{
    public partial class Printer : Form
    {
        Timer timer;
        List<ISchoolClass> classes;
        public Printer()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();

            classes = Program.GetModels().ToList();

            //ExportToExcel(classes, "SchoolTimetable.xlsx", true);//purva
            //ExportToExcel(classes, "SchoolTimetable.xlsx", false);//vtora
            Program.ShowTheCurrentIcon(pictureBox3);
        }
        //-----------------------------------------------------
        public enum BulgarianDaysOfWeek
        {
            ПОНЕДЕЛНИК,
            ВТОРНИК,
            СРЯДА,
            ЧЕТВЪРТЪК,
            ПЕТЪК
        }
        private static void ExportToExcel(List<ISchoolClass> classes, string filePath, bool isPurva)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Програма");

                worksheet.Cells[1, 1].Value = "ЧАС";
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 14;
                worksheet.Column(1).Width = 10;
                worksheet.Cells[1, 2].Value = "ПРОДЪЛЖИ-\nТЕЛНОСТ";
                worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 2].Style.Font.Bold = true;
                worksheet.Cells[1, 2].Style.Font.Size = 8;
                worksheet.Column(2).Width = 30;

                int column = 3;
                foreach (BulgarianDaysOfWeek day in Enum.GetValues(typeof(BulgarianDaysOfWeek)))
                {
                    worksheet.Cells[1, column].Value = day.ToString();
                    worksheet.Cells[1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, column].Style.Font.Bold = true;
                    worksheet.Cells[1, column].Style.Font.Size = 14;
                    worksheet.Column(column).Width = 20;
                    column++;
                }
                if (isPurva)
                    DrawClassesForShift(classes, worksheet, 2, true);
                else
                    DrawClassesForShift(classes, worksheet, 2, false);

                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);
            }
        }

        private static void DrawClassesForShift(List<ISchoolClass> classes, ExcelWorksheet worksheet, int startRow, bool isFirstShift)
        {
            var groupedClasses = classes
            .Where(c => c.IsPurvaSmqna == isFirstShift).OrderBy(x => x.Day)
            .ThenBy(x => x.IsPurvaSmqna).ThenBy(x => x.Num);
            for (int i = 0; i < 7; i++)
            {
                TimeForClockAndText time = new TimeForClockAndText();
                List<int> a;
                if (isFirstShift)
                    a = time.CalculateClassDuration(i + 1, true);
                else
                    a = time.CalculateClassDuration(i + 1, false);
                worksheet.Cells[i + 3, 2].Value = $" от {a[0]:D2}:{a[1]:D2} до {a[2]:D2}:{a[3]:D2}";
                //worksheet.Cells[i + 3, 2].Value = $"{a[0]:D2}:{a[1]:D2}-{a[2]:D2}:{a[3]:D2}";

                worksheet.Cells[i + 3, 1].Value = $"{i + 1}";
                worksheet.Cells[i + 3, 2].Style.Font.Bold = true;
                worksheet.Cells[i + 3, 1].Style.Font.Bold = true;
            }
            int row = startRow;
            if (isFirstShift)
            {
                var mergedCell1 = worksheet.Cells[2, 1, 2, 7];
                mergedCell1.Merge = true;
                mergedCell1.Value = "ПЪРВА СМЯНА";
                mergedCell1.Style.Font.Bold = true;
                mergedCell1.Style.Font.Size = 14;
                mergedCell1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            else
            {
                var mergedCell2 = worksheet.Cells[2, 1, 2, 7];
                mergedCell2.Merge = true;
                mergedCell2.Value = "ВТОРА СМЯНА";
                mergedCell2.Style.Font.Bold = true;
                mergedCell2.Style.Font.Size = 14;
                mergedCell2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            foreach (var classGroup in groupedClasses)
            {
                string classDetails = classGroup.GetClassGradeAndParalelkaForPrinting();
                worksheet.Cells[classGroup.Num + 2, dayToColumnIndex(classGroup.Day)].Value = classDetails;
                row++;

            }
            //for styling the table
            foreach (var cell in worksheet.Cells)
            {
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                cell.Style.WrapText = true;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Font.Size = 18;
                cell.AutoFitColumns(15, 30);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
            //worksheet.Column(1).Width = 10;
            //worksheet.Column(2).Width = 20;

            int column = 3;
            foreach (BulgarianDaysOfWeek day in Enum.GetValues(typeof(BulgarianDaysOfWeek)))
            {
                worksheet.Cells[1, column].Style.Font.Bold = true;
                worksheet.Cells[1, column].Style.Font.Size = 15;
                worksheet.Column(column).Width = 18;
                worksheet.Cells[1, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                column++;
            }
            worksheet.Cells[1, 2].Style.Font.Size = 12;
        }


        private static int dayToColumnIndex(string day)
        {
            switch (day.ToUpper()) // Ensure case-insensitive comparison
            {
                case "ПОНЕДЕЛНИК": return 3;
                case "ВТОРНИК": return 4;
                case "СРЯДА": return 5;
                case "ЧЕТВЪРТЪК": return 6;
                case "ПЕТЪК": return 7;
                default: throw new ArgumentException("Invalid day");
            }
        }

        //-----------------------------------------------------
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
            classes = Program.GetModels().ToList();
            Program.ShowTheCurrentIcon(pictureBox3);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Program.LastForms.Pop().Show();
            this.Dispose();
            this.Close();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }
        private void pictureBoxFirst_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                labelInstructions.Hide();
                string filePath = saveFileDialog.FileName;
                ExportToExcel(classes, filePath, true);
                RunThePrinter(filePath);
                ShowCheck(false);
            }
            else
            {
                ShowCheck(true);
            }
        }

        private void pictureBoxSecond_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                labelInstructions.Hide();
                string filePath = saveFileDialog.FileName;
                ExportToExcel(classes, filePath, false);
                RunThePrinter(filePath);
                ShowCheck(false);
            }
            else
            {
                ShowCheck(true);
            }
        }

        System.Windows.Forms.Timer tempTimer = new System.Windows.Forms.Timer();
        private void ShowCheck(bool thereIsError)
        {
            tempTimer.Interval = 4000;
            tempTimer.Tick += TempTimer_Tick;
            if (!thereIsError)
            {
                pictureBoxCompleteOrError.Image = Properties.Resources.newSolidCheck;
                panel1.BackColor = Color.PaleGreen;
                //labelFeedBack.ForeColor = Color.FromArgb(16, 145, 33);
                labelFeedBack.ForeColor = Color.White;
                labelFeedBack.Text = "ФАЙЛЪТ Е ЗАПАЗЕН";
            }
            else
            {
                pictureBoxCompleteOrError.Image = Properties.Resources.errorCross;
                panel1.BackColor = Color.Red;
                labelFeedBack.ForeColor = Color.White;
                labelFeedBack.Text = "НЕЩО СЕ ОБЪРКА!";
            }
            pictureBoxCompleteOrError.Refresh();
            panel1.Show();
            tempTimer.Start();
        }

        private void TempTimer_Tick(object sender, EventArgs e)
        {
            pictureBoxCompleteOrError.Image = null;
            panel1.Hide();
            tempTimer.Stop();
        }

        private void RunThePrinter(string filePath)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("Желаете ли да принтирате файлът още сега?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    PrintExcelFile(filePath);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Грешка: " + ex.Message);
                }
            }
            else
            {
                labelInstructions.Show();
                System.Windows.Forms.MessageBox.Show("За да принтирате ръчно програмата, моля следвайте инструкциите по-долу!");
            }


        }
        static void PrintExcelFile(string filePath)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (sender, e) =>
                {
                    e.PageSettings.Landscape = true; // Set orientation to landscape
                    e.PageSettings.PaperSize = new PaperSize("A4", 827, 1169); // Set page format to A4

                    Microsoft.Office.Interop.Excel.Application excelApp = null;
                    Microsoft.Office.Interop.Excel.Workbooks workbooks = null;
                    Microsoft.Office.Interop.Excel.Workbook workbook = null;
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

                    try
                    {
                        excelApp = new Microsoft.Office.Interop.Excel.Application();
                        workbooks = excelApp.Workbooks;
                        workbook = workbooks.Open(filePath);
                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                        // Set additional print settings if needed
                        worksheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;

                        workbook.PrintOut();
                        workbook.Save();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Грешка: " + ex.Message);
                    }
                    finally
                    {
                        // Release COM objects to avoid memory leaks
                        if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                        if (workbook != null) Marshal.ReleaseComObject(workbook);
                        if (workbooks != null) Marshal.ReleaseComObject(workbooks);
                        if (excelApp != null)
                        {
                            excelApp.Quit();
                            Marshal.ReleaseComObject(excelApp);
                        }
                    }
                };
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = pd;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    pd.Print();
                    System.Windows.Forms.MessageBox.Show("Файлът се принтира!");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Файлът не е принтиран!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Грешка при принтиране: " + ex.Message);
            }
        }

        private void dEFAULTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ChangeCustomIcon(pictureBox3, false);
            Program.customIconPath = null;
        }

        private void cUSTOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ChoosePathForCustomIcon(contextMenuStrip1);
            Program.ChangeCustomIcon(pictureBox3, true);
        }
    }
}
