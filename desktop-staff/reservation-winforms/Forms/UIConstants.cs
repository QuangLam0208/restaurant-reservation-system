using System.Drawing;

namespace reservation_winforms
{
    public static class UIConstants
    {
        // Kích thước chuẩn của màn hình POS (1024 x 768)
        public static readonly Size PosScreenSize = new Size(1024, 768);

        // Bạn có thể lưu thêm các màu chủ đạo ở đây để dùng chung cho đồng bộ
        public static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185); // Xanh dương
        public static readonly Color BackgroundColor = Color.FromArgb(245, 246, 250); // Xám nhạt
    }
}