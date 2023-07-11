using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace VVX
{
    /// <summary>
    /// A simple class to create/render/save the reflection of any image with fade out
    /// Fadeout only work with files that can be converted to PixelFormat.Format32bppArgb;
    /// for other images, it simply produces a uniformly faded reflection.
    /// In either case, the value of 'byMaxOpacity' (0 to 255) determines how the image 
    /// is faded. NOTE: 0 is completely transparent, 255 is completely opaque
    /// </summary>
    class VVX_ImageTools
    {
        public enum Mirror
        {
            Above,
            Below,
            Left,
            Right
        }
        
        private string msErrorMsg = "";
        private string msEOL = Environment.NewLine;

        public string ErrorMsg
        {
            get { return msErrorMsg; }
            set { msErrorMsg = value; }
        }
        private string msFile = "Display.png";
        private string msFileOut = "Display.Reflection.png";

        public string FileOut
        {
            get { return msFileOut; }
            set { msFileOut = value; mbSaveReflection = (msFileOut.Length > 0); }
        }
        private RotateFlipType menRotateFlipType = RotateFlipType.RotateNoneFlipY;
        private bool mbGradientFade = true;
        private bool mbDrawOriginal = true;
        private bool mbWantReflection = true;
        private bool mbDrawReflection = true;
        private bool mbSaveReflection = !true;
        private bool mbOkToReplace = true;
        private int mnVerticalOffset = 0;
        private int mnHorizontalOffset = 0;
        private byte mbyInitialOpacity = 128;    //of reflection
        private bool mbReverseFade = false;
        private int mnWidthOfBoxAroundOriginal = 2;

        private bool mbMakeTransparent = false;
        private Point mptMakeTransparentColorAt = new Point(-1, -1);
        private Color mColorToMakeTransparent = Color.Black;    //default
        private bool mbHaveMakeTransparentColor = false;

        /// <summary>
        /// Must be true or an existing file will not be replaced
        /// </summary>
        public bool OkToReplace
        {
            get { return mbOkToReplace; }
            set { mbOkToReplace = value; }
        }

        /// <summary>
        /// If true, then all pixels with a color that matches MakeTransparentColor
        /// will be made transparent. Either set the MakeTransparentColor directly 
        /// or provide it coordinates using MakeTransparentColorAtPoint.
        /// </summary>
        public bool MakeTransparent
        {
            get { return mbMakeTransparent; }
            set { mbMakeTransparent = value; }
        }
        /// <summary>
        /// All pixels with this color will be made transparent.
        /// NOTE: The MakeTransparent must be 'true' for this to work
        /// </summary>
        public Color MakeTransparentColor
        {
            get { return mColorToMakeTransparent; }
            set { mColorToMakeTransparent = value; }
        }
        /// <summary>
        /// All pixels with the color at this point will be made transparent.
        /// NOTE: The MakeTransparent must be 'true' for this to work
        /// </summary>
        public Point MakeTransparentColorAtPoint
        {
            get { return mptMakeTransparentColorAt; }
            set { mptMakeTransparentColorAt = value; }
        }

        public bool ReverseFade
        {
            get { return mbReverseFade; }
            set { mbReverseFade = value; }
        }

        //******************************************************************************

        /// <summary>
        /// Disabled constructor
        /// </summary>
        private VVX_ImageTools()    //basically to block this constructor
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sFile">file from which Bitmap is loaded</param>
        /// <param name="bFlipVertically">If false, image flipped horizontally</param>
        public VVX_ImageTools(string sFile)
        {
            DoInit(sFile, 128, "");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sFile">file from which Bitmap is loaded</param>
        /// <param name="byMaxOpacity">Maximum opacity of the reflected image</param>
        public VVX_ImageTools(string sFile, byte byMaxOpacity)
        {
            DoInit(sFile, byMaxOpacity, "");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sFile">file from which Bitmap is loaded</param>
        /// <param name="byMaxOpacity">Maximum opacity of the reflected image</param>
        /// <param name="sFileOut">file in which the resulting bitmap is to be stored</param>
        public VVX_ImageTools(string sFile, byte byMaxOpacity, string sFileOut)
        {
            DoInit(sFile, byMaxOpacity, sFileOut);
        }

        /// <summary>
        /// Shared initialization for the various constructors
        /// </summary>
        /// <param name="sFile"></param>
        /// <param name="byMaxOpacity"></param>
        /// <param name="sFileOut"></param>
        private void DoInit(string sFile, byte byMaxOpacity, string sFileOut)
        {
            this.msFile = sFile;
            this.mbyInitialOpacity = byMaxOpacity;
            if(sFileOut.Length > 0)
                this.msFileOut = sFileOut;
        }

        /// <summary>
        /// Displays image and/or its reflection
        /// </summary>
        /// <param name="gdi">Graphics instance where the image and/or reflection will be rendered</param>
        /// <param name="nPosX">X position of the location of the rendered image</param>
        /// <param name="nPosY">Y position of the location of the rendered image</param>
        /// <param name="enMirror">Specifies how (and where) the reflection will be rendered</param>
        /// <returns>if 'false', be sure to check the ErrorMsg property </returns>
        public bool DoShowImageAndReflection(Graphics gdi, int nPosX, int nPosY, Mirror enMirror)
        {
            bool bRet = true;

            if (msFile.Length == 0)
                return false;

            try
            {
                switch (enMirror)
                {
                    case Mirror.Below:
                        this.menRotateFlipType = RotateFlipType.RotateNoneFlipY;
                        this.mbReverseFade = false;
                        break;
                    case Mirror.Above:
                        this.menRotateFlipType = RotateFlipType.RotateNoneFlipY;
                        this.mbReverseFade = true;
                        break;
                    case Mirror.Right:
                        this.menRotateFlipType = RotateFlipType.RotateNoneFlipX;
                        this.mbReverseFade = false;
                        break;
                    case Mirror.Left:
                        this.menRotateFlipType = RotateFlipType.RotateNoneFlipX;
                        this.mbReverseFade = true;
                        break;
                }

                // Create the Bitmap object and load it with the image.
                Bitmap bmpOriginal = new Bitmap(msFile);
                int iWidth = bmpOriginal.Width;
                int iHeight = bmpOriginal.Height;

                if(((long)iWidth * iHeight ) > 512000)
                {
                    this.DoUpdateErrorMsg("This file is too big");
                    return false;
                }

                // in case we have make certain pixels transparent get the color
                if(this.mbMakeTransparent && !this.mbHaveMakeTransparentColor)
                    this.DoInitColorToMakeTransparent(bmpOriginal);

                // Now draw the semitransparent bitmap image.
                if (this.mbDrawOriginal)
                    gdi.DrawImage(bmpOriginal, nPosX, nPosY, iWidth, iHeight);

                if (this.mbWantReflection)
                {
                    Bitmap bmpReflection = new Bitmap(msFile);
                    bmpReflection.RotateFlip(this.menRotateFlipType);

                    if (this.mbGradientFade)
                    {
                        if (bmpReflection.PixelFormat != PixelFormat.Format32bppArgb)
                        {
                            DoConvertToFormat32bppArgb(bmpOriginal, ref bmpReflection);
                            if (true)
                            {
                                string sConvertedFile = msFile+".32bppArgb.png";
                                this.SaveAs(bmpReflection, sConvertedFile);
                                this.DoUpdateErrorMsg(sConvertedFile + " will work faster!");
                            }
                            bmpReflection.RotateFlip(this.menRotateFlipType);
                        }

                        //--- now fade out the reflected image
                        if (bmpReflection.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            //Source: http://www.bobpowell.net/lockingbits.htm
                            //Format32BppArgb Given X and Y coordinates,  the address of the first element in the 
                            // pixel is Scan0+(y * stride)+(x*4). This Points to the blue byte. The following 
                            // three bytes contain the green, red and alpha bytes. 

                            Rectangle rc = new Rectangle(0, 0, iWidth, iHeight);
                            BitmapData bmd = bmpReflection.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            /*
                                The BitmapData class contains the following important properties;

                                * Scan0         The address in memory of the fixed data array 
                                * Stride        The width, in bytes, of a single row of pixel data. This width is 
                                                a multiple, or possiblysub-multiple, of the pixel dimensions of 
                                                the image and may be padded out to include a few more bytes. 
                                * PixelFormat   The actual pixel format of the data. This is important for finding the right bytes 
                                * Width         The width of the locked image 
                                * Height        The height of the locked image 
                             */

                            //--- now set the alpha
                            unsafe
                            {
                                //NOTE: For this to work in VS2005, in Project > Properties > Build
                                // you must check "Allow Unsafe Code"
                                int PixelSize = 4;
                                byte byOpacity = mbyInitialOpacity;
                                byte byDelta = (byte)(mbyInitialOpacity / bmd.Height);
                                if (byDelta < 1)
                                    byDelta = 1;
                                
                                if (this.menRotateFlipType == RotateFlipType.RotateNoneFlipY)
                                {
                                    if (mbReverseFade)
                                    {
                                        if (((int)mbyInitialOpacity - byDelta*bmd.Height) > 0)
                                            byOpacity = (byte)((int)mbyInitialOpacity - byDelta*bmd.Height);
                                        else
                                            byOpacity = 0;
                                    }

                                    for (int y = 0; y < bmd.Height; y++)
                                    {
                                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                                        for (int x = 0; x < bmd.Width; x++)
                                        {
                                            byte byB = row[x * PixelSize];
                                            byte byR = row[x * PixelSize + 1];
                                            byte byG = row[x * PixelSize + 2];
                                            byte byA = row[x * PixelSize + 3];
                                            //byte Blue
                                            //set the blue component of a 32 bit per pixel image to 255
                                            //row[x * PixelSize] = 255;
                                            //set the green component of a 32 bit per pixel image to 255
                                            //row[x * PixelSize + 1] = 255;
                                            //set the red component of a 32 bit per pixel image to 255
                                            //row[x * PixelSize + 2] = 255;
                                            //set the red component of a 32 bit per pixel image to 255
                                            if (byB != 0 || byG != 0 || byR != 0)
                                            {
                                                if (byA > 100)
                                                    row[x * PixelSize + 3] = byOpacity;
                                            }
                                        }

                                        if (!mbReverseFade)
                                        {
                                            if (byOpacity >= byDelta)
                                            {
                                                byOpacity -= byDelta;
                                            }
                                        }
                                        else
                                        {
                                            if (byOpacity < this.mbyInitialOpacity)
                                            {
                                                byOpacity += byDelta;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    byDelta = (byte)(mbyInitialOpacity / bmd.Width);
                                    if (byDelta < 1)
                                        byDelta = 1;
                                    for (int y = 0; y < bmd.Height; y++)
                                    {
                                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                                        if (!mbReverseFade)
                                            byOpacity = mbyInitialOpacity;
                                        else
                                        {
                                            if (((int)mbyInitialOpacity - byDelta*bmd.Width) > 0)
                                                byOpacity = (byte)((int)mbyInitialOpacity - byDelta*bmd.Width);
                                            else
                                                byOpacity = 0;
                                        }
                                        for (int x = 0; x < bmd.Width; x++)
                                        {
                                            byte byB = row[x * PixelSize];
                                            byte byR = row[x * PixelSize + 1];
                                            byte byG = row[x * PixelSize + 2];
                                            byte byA = row[x * PixelSize + 3];

                                            if (byB != 0 || byG != 0 || byR != 0)
                                            {
                                                if (byA > 100)
                                                    row[x * PixelSize + 3] = byOpacity;
                                            }

                                            if (!mbReverseFade)
                                            {
                                                if (byOpacity >= byDelta)
                                                {
                                                    byOpacity -= byDelta;
                                                }
                                            }
                                            else
                                            {
                                                if (byOpacity < this.mbyInitialOpacity)
                                                {
                                                    byOpacity += byDelta;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //now unlock
                            bmpReflection.UnlockBits(bmd);
                        }

                        if (this.mbSaveReflection)
                        {
                            bRet = SaveAs(bmpReflection, this.msFileOut);
                        }


                    } // end of bGradientFade

                    if (this.mbDrawReflection)
                    {
                        int nNewX = nPosX + this.mnHorizontalOffset;
                        int nNewY = nPosY + this.mnVerticalOffset;

                        if (this.mbDrawOriginal) // then offset appropriately
                        {
                            switch (menRotateFlipType)
                            {
                                case RotateFlipType.RotateNoneFlipY:
                                    //case RotateFlipType.Rotate90FlipY:
                                    //case RotateFlipType.Rotate180FlipY:
                                    //case RotateFlipType.Rotate270FlipY:
                                    if (mbReverseFade)
                                        nNewY -= iHeight;
                                    else
                                        nNewY += iHeight;
                                    break;
                                case RotateFlipType.RotateNoneFlipX: //same as RotateFlipType.Rotate180FlipY
                                case RotateFlipType.Rotate90FlipX:   //same as RotateFlipType.Rotate270FlipY
                                case RotateFlipType.Rotate270FlipX:  //same as RotateFlipType.Rotate90FlipY
                                    if (mbReverseFade)
                                        nNewX -= iWidth;
                                    else
                                        nNewX += iWidth;
                                    break;

                                case RotateFlipType.RotateNoneFlipXY:
                                    nNewY += iHeight;
                                    break;
                            }
                        }

                        if (bmpReflection.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            gdi.DrawImage(bmpReflection, nNewX, nNewY, iWidth, iHeight);
                        }
                        else
                        {
                            // Initialize the color matrix.
                            // Note the value fFade in row 4, column 4.
                            float fFade = (float)mbyInitialOpacity / (float)255;
                            float[][] matrixItems ={ 
                           new float[] {1, 0, 0, 0, 0},
                           new float[] {0, 1, 0, 0, 0},
                           new float[] {0, 0, 1, 0, 0},
                           new float[] {0, 0, 0, fFade, 0}, 
                           new float[] {0, 0, 0, 0, 1}};
                            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

                            // Create an ImageAttributes object and set its color matrix.
                            ImageAttributes imageAtt = new ImageAttributes();
                            imageAtt.SetColorMatrix(
                               colorMatrix,
                               ColorMatrixFlag.Default,
                               ColorAdjustType.Bitmap);

                            gdi.DrawImage(bmpReflection,
                               new Rectangle(nNewX, nNewY, iWidth, iHeight),  // destination rectangle
                               0.0f,                          // source rectangle x 
                               0.0f,                          // source rectangle y
                               iWidth,                        // source rectangle width
                               iHeight,                       // source rectangle height
                               GraphicsUnit.Pixel,
                               imageAtt);
                        }
                    }
                } // end of if(bWantReflection)

                if (this.msErrorMsg.Length > 0)
                    Debug.WriteLine(this.msErrorMsg);

                if (this.mbDrawOriginal)
                {
                    //--- draw a light gray box around the original
                    if (mnWidthOfBoxAroundOriginal != 0)
                    {
                        Rectangle rc = new Rectangle(nPosX, nPosY, iWidth, iHeight);
                        //rc.Inflate(mnWidthOfBoxAroundOriginal, mnWidthOfBoxAroundOriginal);
                        gdi.DrawRectangle(new Pen(Color.LightGray, mnWidthOfBoxAroundOriginal), rc);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                bRet = false;
            }
            return bRet;
        }

        private bool SaveAs(Bitmap bmpReflection, string sOutputFilename)
        {
            bool bRet = true;
            try
            {
                if (sOutputFilename.ToUpper() == this.msFile.ToUpper())
                {
                    this.DoUpdateErrorMsg("ERROR: Output file MUST be different from Input file");
                    return false;
                }

                FileInfo fiOutputFile = new FileInfo(sOutputFilename);
                //determine the image format from the file name
                ImageFormat imgFmtWant = ImageFormat.Png;
                switch(fiOutputFile.Extension.ToUpper())
                {
                    case ".BMP"  : imgFmtWant = ImageFormat.Bmp; break;
                    case ".EMF"  : imgFmtWant = ImageFormat.Emf; break;
                    case ".EXF"  :
                    case ".EXIF" : imgFmtWant = ImageFormat.Exif; break;
                    case ".GIF"  : imgFmtWant = ImageFormat.Gif; break;
                    case ".ICO"  : imgFmtWant = ImageFormat.Icon; break;
                    case ".JPEG" : imgFmtWant = ImageFormat.Jpeg; break;
                    case ".PNG"  : imgFmtWant = ImageFormat.Png; break;
                    case ".TIF":
                    case ".TIFF" : imgFmtWant = ImageFormat.Tiff; break;
                    case ".WMF"  : imgFmtWant = ImageFormat.Wmf; break;
                    default: // none of the above, so add PNG to the name
                        sOutputFilename += ".png";
                        this.DoUpdateErrorMsg("WARNING: Output file name modified; Extension '.png' added.");
                        break;
                }
                fiOutputFile = new FileInfo(sOutputFilename);
                bool bOkToSave = true;
                if (fiOutputFile.Exists)
                    if (this.mbOkToReplace)
                    {
                        this.DoUpdateErrorMsg("WARNING: AutoSave Output file replaces another file with the same name.");
                    }
                    else
                    {
                        bOkToSave = false;
                    }

                if(bOkToSave)
                    bmpReflection.Save(sOutputFilename, imgFmtWant);

                this.mbOkToReplace = false; //so that we only replace ONCE until caller resets it
            }
            catch (Exception ex)
            {
                this.msErrorMsg += this.msEOL + ex.ToString();
                bRet = false;
            }
            return bRet;
        }

        /// <summary>
        /// Adds the message to the error string
        /// </summary>
        /// <param name="sMsg"></param>
        private void DoUpdateErrorMsg(string sMsg)
        {
            if (this.msErrorMsg.Length > 0)
                this.msErrorMsg += this.msEOL;
            this.msErrorMsg += sMsg;
        }

        /// <summary>
        /// Converts a bitmap to a 32-bit ARGB compatible bitmap and sets the Alpha
        /// value depending on the 
        /// </summary>
        /// <param name="bmpSrc"></param>
        /// <param name="bmpTgt"></param>
        /// <returns></returns>
        public bool DoConvertToFormat32bppArgb(Bitmap bmpSrc, ref Bitmap bmpTgt)
        {
            bool bRet = false;

            try
            {
                PixelFormat pixFmtSrc = bmpSrc.PixelFormat;
                PixelFormat pixFmtTgt = PixelFormat.Format32bppArgb;
                //Debug.WriteLine(pixFmtSrc.ToString());

                if (pixFmtSrc != PixelFormat.Format32bppRgb)
                {
                    int iWidth = bmpSrc.Width;
                    int iHeight = bmpSrc.Height;
                    // create a bitmap with the same size but desired format
                    bmpTgt = new Bitmap(bmpSrc.Width, bmpSrc.Height, pixFmtTgt);

                    //Ref: http://www.bobpowell.net/lockingbits.htm
                    // lock all the bits
                    Rectangle rc = new Rectangle(0, 0, iWidth, iHeight);
                    BitmapData bmdTgt = bmpTgt.LockBits(rc, ImageLockMode.ReadOnly, pixFmtTgt);

                    unsafe
                    {
                        //NOTE: For this to work in VS2005, in Project > Properties > Build
                        // you must check "Allow Unsafe Code"
                        //--- loop through each scan line and COPY each pixel from current
                        //    format to desired format, e.g., Format32bppRgb 
                        int PixelSize = 4;
                        for (int y = 0; y < bmdTgt.Height; y++)
                        {
                            byte* row = (byte*)bmdTgt.Scan0 + (y * bmdTgt.Stride);
                            for (int x = 0; x < bmdTgt.Width; x++)
                            {
                                //---get the color from the source
                                Color clr = bmpSrc.GetPixel(x, y);
                                //---set color attributes in the target
                                // Each of the 4 bytes of this 32-bit pixel must be set
                                int xRef = x * PixelSize;
                                row[xRef + 0] = clr.B;
                                row[xRef + 1] = clr.G;
                                row[xRef + 2] = clr.R;
                                if (mbMakeTransparent && (clr == this.mColorToMakeTransparent))
                                    row[xRef + 3] = 0;      // 0 is 100% transparent
                                else
                                    row[xRef + 3] = 255;   // 255 is 100% opaque
                            }
                        }
                    }
                    //now unlock all the bits we locked earlier
                    bmpTgt.UnlockBits(bmdTgt);
                }

                bRet = true;
            }
            catch (Exception ex)
            {
                this.msErrorMsg += this.msEOL + ex.ToString();
                bRet = false;
            }
            return bRet;
        }

        /// <summary>
        /// Obtains the color to be used to mark the Alpha byte of a pixel as transparent.
        /// It uses the MakeTransparentColorAtPoint property to determine the pixel which
        /// contains the desired color.
        /// NOTE: The MakeTransparent must be 'true' for this to work
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public bool DoInitColorToMakeTransparent(Bitmap bmp)
        {
            int iX = mptMakeTransparentColorAt.X;
            int iY = mptMakeTransparentColorAt.Y;

            return DoInitColorToMakeTransparent(bmp, iX, iY);
        }

        /// <summary>
        /// Obtains the color to be used to mark the Alpha byte of a pixel as transparent.
        /// It uses iX and iY determine the pixel which contains the desired color that will
        /// be made transparent
        /// NOTE: The MakeTransparent must be 'true' for this to work
        /// </summary>
        /// <param name="bmp">Bitmap used to determine color</param>
        /// <param name="iX">X position of the pixel whose color is used to make all such pixels transparent</param>
        /// <param name="iY">Y position of the pixel whose color is used to make all such pixels transparent</param>
        /// <returns></returns>
        public bool DoInitColorToMakeTransparent(Bitmap bmp, int iX, int iY)
        {
            if (this.mbMakeTransparent)
            {
                if ((iX < 0 || iX >= bmp.Width) 
                 || (iY < 0 || iY >= bmp.Height))
                    this.mbMakeTransparent = false;
                else
                {
                    this.mColorToMakeTransparent = bmp.GetPixel(iX, iY);
                    this.mbHaveMakeTransparentColor = true;
                }
            }

            return this.mbMakeTransparent;
        }

        /// <summary>
        /// Initis the color that will be used to make transparent all similarly colored pixels
        /// NOTE: The MakeTransparent must be 'true' for this to work
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool DoInitColorToMakeTransparent(Color color)
        {
            this.mColorToMakeTransparent = color;
            this.mbHaveMakeTransparentColor = true;
            return this.mbMakeTransparent;
        }

        private bool MatchesTransparentTarget(Color clr)
        {
            bool bRet = false;
            if (clr.R == this.mColorToMakeTransparent.R
             && clr.G == this.mColorToMakeTransparent.G
             && clr.B == this.mColorToMakeTransparent.B)
                bRet = true;

            return bRet;
        }

        //public bool DoMakeTransparent(Bitmap bmpSrc, out Bitmap bmpTgt, int iX, int iY)
        //{
        //    bmpTgt = null;
        //    DoInitColorToMakeTransparent(bmpSrc, iX, iY);
        //    if (bmpSrc.PixelFormat != PixelFormat.Format32bppArgb)
        //    {
        //        DoConvertToFormat32bppArgb(bmpSrc, ref bmpTgt);
        //    }
        //    return DoMakeTransparent(bmpSrc, out bmpTgt);
        //}

        public bool DoMakeTransparent(Bitmap bmpSrc, out Bitmap bmpTgt)
        {
            bool bRet = false;
            bmpTgt = null;

            try
            {
                PixelFormat pixFmtTgt = PixelFormat.Format32bppArgb;

                if (bmpSrc.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    DoConvertToFormat32bppArgb(bmpSrc, ref bmpTgt);
                }

                PixelFormat pixFmtSrc = bmpSrc.PixelFormat;
                //Debug.WriteLine(pixFmtSrc.ToString());

                if (pixFmtSrc != PixelFormat.Format32bppRgb)
                {
                    int iWidth = bmpSrc.Width;
                    int iHeight = bmpSrc.Height;
                    // create a bitmap with the same size but desired format
                    bmpTgt = new Bitmap(bmpSrc.Width, bmpSrc.Height, pixFmtTgt);

                    //Ref: http://www.bobpowell.net/lockingbits.htm
                    // lock all the bits
                    Rectangle rc = new Rectangle(0, 0, iWidth, iHeight);
                    BitmapData bmdTgt = bmpTgt.LockBits(rc, ImageLockMode.ReadOnly, pixFmtTgt);

                    unsafe
                    {
                        //NOTE: For this to work in VS2005, in Project > Properties > Build
                        // you must check "Allow Unsafe Code"
                        //--- loop through each scan line and COPY each pixel from current
                        //    format to desired format, e.g., Format32bppRgb 
                        int PixelSize = 4;
                        for (int y = 0; y < bmdTgt.Height; y++)
                        {
                            byte* row = (byte*)bmdTgt.Scan0 + (y * bmdTgt.Stride);
                            for (int x = 0; x < bmdTgt.Width; x++)
                            {
                                //---get the color from the source
                                Color clr = bmpSrc.GetPixel(x, y);
                                //---set color attributes in the target
                                // Each of the 4 bytes of this 32-bit pixel must be set
                                int xRef = x * PixelSize;
                                row[xRef + 0] = clr.B;
                                row[xRef + 1] = clr.G;
                                row[xRef + 2] = clr.R;
                                if (mbMakeTransparent && this.MatchesTransparentTarget(clr))
                                {
                                    row[xRef + 2] = 255;
                                    row[xRef + 3] = 0;      // 0 is 100% transparent
                                }
                                else
                                    row[xRef + 3] = 255;   // 255 is 100% opaque
                            }
                        }
                    }
                    //now unlock all the bits we locked earlier
                    bmpTgt.UnlockBits(bmdTgt);
                }

                bRet = true;
            }
            catch (Exception ex)
            {
                this.msErrorMsg += this.msEOL + ex.ToString();
                bRet = false;
            }
            return bRet;
        }

    }
}
