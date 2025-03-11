namespace Test_Assistant.pagesModels
{
    public class ScreenSelection : Form
    {
        private Point startPoint;
        private Point endPoint;
        private bool isSelecting = false;
        private Rectangle selectionRectangle = Rectangle.Empty;
        private TaskCompletionSource<List<Point>> selectionTask;

        public ScreenSelection()
        {
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.LightGray;
            this.Opacity = 0.5;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.Paint += OnPaint;
        }

        public Task<List<Point>> StartSelection()
        {
            selectionTask = new TaskCompletionSource<List<Point>>();
            this.Show();
            return selectionTask.Task;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = e.Location;
                selectionRectangle = new Rectangle(e.Location, Size.Empty);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                endPoint = e.Location;
                selectionRectangle = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                );
                Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (isSelecting && e.Button == MouseButtons.Left)
            {
                isSelecting = false;
                endPoint = e.Location;
                selectionTask.SetResult(new List<Point> { startPoint, endPoint });
                this.Close();
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (isSelecting)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }
    }
}