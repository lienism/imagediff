using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ImageDiff
{
    public partial class Diff_Form : Form
    {
        Mat isLeftMat;
        Mat isRightMat;
        public Diff_Form()
        {
            InitializeComponent();
        }

        private void Load_Button_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                this.isLeftMat = Cv2.ImRead(openFileDialog1.FileName);
                this.isRightMat = Cv2.ImRead(openFileDialog2.FileName);
                pictureBox3.Image = CompareImage(this.isLeftMat, this.isRightMat);
                pictureBox2.Image = isRightMat.ToBitmap();
                pictureBox1.Image = isLeftMat.ToBitmap();
            }
        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "画像ファイル(*.jpg)|*.jpg|画像ファイル(*.png)|*.png";
            string[] Filesaved = { ".jpg", ".png" };
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap Savebmp = new Bitmap(pictureBox3.Image);
                Savebmp.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// 比較して結果を出す
        /// </summary>
        /// <param name="Left">左の画像</param>
        /// <param name="Right">右の画像</param>
        /// <returns>比較した結果</returns>
        private Bitmap CompareImage(Mat Left, Mat Right)
        {
            Mat Result = new Mat();
            Cv2.Absdiff(Left, Right, Result);
            Result = BlurProcess(Result);
            Result = Diff_Red_Draw(Result, Left);
            return Result.ToBitmap();
        }
        /// <summary>
        /// ノイズ削除
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Mat BlurProcess(Mat src)
        {
            Cv2.Dilate(src, src, new Mat(new OpenCvSharp.Size(11, 11), MatType.CV_8UC1));
            Cv2.Erode(src, src, new Mat(new OpenCvSharp.Size(11, 11), MatType.CV_8UC1));
            return src;
        }

        /// <summary>
        /// 差分に赤色を描きます
        /// </summary>
        /// <param name="src">差分のイメージ</param>
        /// <returns>赤色の差分表示</returns>
        private Mat Diff_Red_Draw(Mat src, Mat left)
        {
            Mat Original = left;
            Mat result = new Mat();
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hi;
            src = Gray(src);
            Cv2.Threshold(src, src, 0, 255, ThresholdTypes.Otsu);

            Cv2.FindContours(src, out contours, out hi, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            result = Diff_Draw(contours, Original);

            return result;
        }

        private Mat Diff_Draw(OpenCvSharp.Point[][] contours, Mat src)
        {
            RotatedRect RoRect;
            Rect Rect;
            for(int i = 0; i < contours.Length; i++)
            {
                RoRect = Cv2.MinAreaRect(contours[i]);
                Rect = RoRect.BoundingRect();
                Cv2.Rectangle(src, Rect, new Scalar(0, 0, 255), 4);
            }
            return src;
        }


        /// <summary>
        /// Gray化する関数
        /// </summary>
        /// <param name="src">オリジナル画像</param>
        /// <returns>グレー画像</returns>
        private Mat Gray(Mat src)
        {
            Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
            return src;
        }
    }
}
