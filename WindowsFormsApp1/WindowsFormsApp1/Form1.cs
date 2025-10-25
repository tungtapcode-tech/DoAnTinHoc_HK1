using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void GhiFile(DataTable table, string outputPath)
        {
            using (var writer = new StreamWriter(outputPath))
            {
                // Write header
                writer.WriteLine("bookID,title,author,average_rating,num_pages,ratings_count");
                // Write rows
                foreach (DataRow row in table.Rows)
                {
                    writer.WriteLine($"{row["bookID"]},{row["title"]},{row["author"]},{row["average_rating"]},{row["num_pages"]},{row["ratings_count"]}");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //chuyển đổi 
            string inputPath = "books.csv";
            var table = new DataTable();//var dùng thay thế được cho DataTable
            table.Columns.AddRange(new DataColumn[] {//AddRange thêm hàng loạt, Add thì không
                new DataColumn("bookID",typeof(int)),
                new DataColumn("title"),
                new DataColumn("author"),
                new DataColumn("average_rating",typeof(double)),
                new DataColumn("num_pages",typeof(int)),
                new DataColumn("ratings_count",typeof(int))
            });

            try
            {
                string[] lines = File.ReadAllLines(inputPath);
                bool skipHeader = true;
                foreach (string line in lines)
                {
                    if (skipHeader)
                    {
                        skipHeader = false;
                        continue;
                    }
                    string[] parts = line.Split(',');//tách các cột (,) thành các phần tử
                    if (parts.Length < 9) continue;

                    //bỏ qua các lỗi ký tự trong bookID,avgRating,ratingsCount,numPages)
                    if (!int.TryParse(parts[0], out int bookID)) 
                        continue;
                    if (!double.TryParse(parts[3], out double avgRating)) 
                        continue;
                    if (!int.TryParse(parts[8], out int ratingsCount)) 
                        continue;
                    if (!int.TryParse(parts[7].Trim(), out int numPages))
                        continue;
                    string title = parts[1];
                    string authors = parts[2];


                    //bỏ qua không có đánh giá và trang sách bằng 0
                    if (ratingsCount == 0 || numPages == 0)
                        continue;
                    table.Rows.Add(bookID, title, authors, avgRating, numPages, ratingsCount);
                }
                dataGridView1.DataSource = table; 
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                MessageBox.Show("Đọc sách thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
            string outputPath = "books_clean.csv";
            GhiFile(table, outputPath);
        }

        
    }
}