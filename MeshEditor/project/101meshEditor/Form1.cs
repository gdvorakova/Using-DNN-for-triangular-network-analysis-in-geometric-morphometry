using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Scene3D;
using Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace _101meshEditor
{
    public partial class Form1 : Form
    {
        static readonly string rev = Util.SetVersion( "$Rev: 507 $" );

        /// <summary>
        /// Scene read from file.
        /// </summary>
        SceneBrep scene = new SceneBrep();

        /// <summary>
        /// Scene center point.
        /// </summary>
        Vector3 center = Vector3.Zero;

        /// <summary>
        /// Scene diameter.
        /// </summary>
        float diameter = 4.0f;

        float near = 0.1f;
        float far = 5.0f;

        /// <summary>
        /// Global light source.
        /// </summary>
        Vector3 light = new Vector3( -2, 1, 1 );

        /// <summary>
        /// Point in the 3D scene pointed out by an user, or null.
        /// </summary>
        Vector3? spot = null;

        Vector3? pointOrigin = null;
        Vector3 pointTarget;
        Vector3 eye;

        /// <summary>
        /// Does the pointing ray need recalculation?
        /// </summary>
        bool pointDirty = false;

        /// <summary>
        /// Frustum vertices, 0 or 8 vertices
        /// </summary>
        List<Vector3> frustumFrame = new List<Vector3>();

        /// <summary>
        /// GLControl guard flag.
        /// </summary>
        bool loaded = false;

        /// <summary>
        /// Associated Trackball instance.
        /// </summary>
        Trackball tb = null;

        /// <summary>
        /// Global ToolTip instance (for reuse).
        /// </summary>
        ToolTip tt = new ToolTip();

        /// <summary>
        /// Vertices currently selected by an user.
        /// </summary>
        Selection selectedPoints;
        
        delegate void WriteBrepHandler ( StreamWriter writer, Scene3D.SceneBrep scene, Selection selection,
            IProgress<string> progress, CancellationToken token, out List<int> trianglePointers, out List<int> vertexPointers );

        CancellationTokenSource cts;

        WriteBrepHandler handler;

        bool ringSelect = false;
        bool triangleSelect = false;
        bool vertexSelect = false;


        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Cursor savedCursor;

        int pointLogNumber = 1;
        StreamWriter pointLogFile;

        CommandManager commandManager;

        public Form1 ()
        {
            InitializeComponent();

            string param;
            string branch;
            Program.InitParams( out param, out branch );
            textParam.Text = param ?? "";
            Text += " (" + rev + ") '" + branch + '\'';

            // Trackball:
            tb = new Trackball( center, diameter );

            InitShaderRepository();

            //Set the timer tick event
            timer.Interval = 4000;
            timer.Tick += delegate
            {
                label_percentageReport.Text = String.Empty;
                timer.Stop();
            };

        }

        private void glControl1_Load ( object sender, EventArgs e )
        {
            InitOpenGL();
            UpdateParams( textParam.Text );
            tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );

            loaded = true;
            Application.Idle += new EventHandler( Application_Idle );
        }

        private void glControl1_Resize ( object sender, EventArgs e )
        {
            if ( !loaded ) return;

            tb.GLsetupViewport( glControl1.Width, glControl1.Height, near, far );
            glControl1.Invalidate();
        }

        private void glControl1_Paint ( object sender, PaintEventArgs e )
        {
            Render();
        }

        private void checkVsync_CheckedChanged ( object sender, EventArgs e )
        {
            glControl1.VSync = checkVsync.Checked;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Open Scene File";
            ofd.Filter = "Wavefront OBJ Files|*.obj;*.obj.gz" + "|Polygon File Format|*.ply" +
                         "|All scene types|*.obj;*.ply";
            ofd.FilterIndex = 1;
            ofd.FileName = "";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string ext = Path.GetExtension(ofd.FileName);

            StanfordPly plyReader;
            WavefrontObj objReader;

            if (ext == ".ply")
            {
                plyReader = new StanfordPly();
                if (plyReader.ReadBrep(ofd.FileName, scene) == -1)
                    MessageBox.Show("Invalid .ply file");
            }
            else
            {
                objReader = new WavefrontObj();
                objReader.MirrorConversion = false;
                objReader.TextureUpsideDown = checkOrientation.Checked;
                objReader.ReadBrep(ofd.FileName, scene);
            }

            scene.BuildCornerTable();
            diameter = scene.GetDiameter(out center);
            scene.GenerateColors(12);
            scene.ComputeNormals();

            UpdateParams(textParam.Text);
            tb.Center = center;
            tb.Diameter = diameter;
            SetLight(diameter, ref light);
            tb.Reset();

            labelFile.Text = string.Format("{0} @ {1} vertices, {2} edges ({3}), {4} faces",
                                            ofd.SafeFileName, scene.Vertices,
                                            scene.statEdges, scene.statShared,
                                            scene.Triangles);
            PrepareDataBuffers();

            glControl1.Invalidate();

            // Supports point selection
            selectedPoints = new Selection(scene);
            commandManager = new CommandManager();


    }

        private void buttonLoadTexture_Click ( object sender, EventArgs e )
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Open Image File";
            ofd.Filter = "Bitmap Files|*.bmp" +
                "|Gif Files|*.gif" +
                "|JPEG Files|*.jpg" +
                "|PNG Files|*.png" +
                "|TIFF Files|*.tif" +
                "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

            ofd.FilterIndex = 6;
            ofd.FileName = "";
            if ( ofd.ShowDialog() != DialogResult.OK )
                return;

            Bitmap inputImage = (Bitmap)Image.FromFile( ofd.FileName );

            if ( texName == 0 )
                texName = GL.GenTexture();

            GL.BindTexture( TextureTarget.Texture2D, texName );

            Rectangle rect = new Rectangle( 0, 0, inputImage.Width, inputImage.Height );
            BitmapData bmpData = inputImage.LockBits( rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, inputImage.Width, inputImage.Height, 0,
                           OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bmpData.Scan0 );
            inputImage.UnlockBits( bmpData );
            inputImage.Dispose();
            inputImage = null;

            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
        }

        private void textParam_KeyPress ( object sender, System.Windows.Forms.KeyPressEventArgs e )
        {
            if ( e.KeyChar == (char)Keys.Enter )
            {
                e.Handled = true;
                UpdateParams( textParam.Text );
            }
        }

        private void Form1_FormClosing ( object sender, FormClosingEventArgs e )
        {
            DestroyTexture( ref texName );

            if ( VBOid != null &&
                 VBOid[ 0 ] != 0 )
            {
                GL.DeleteBuffers( 2, VBOid );
                VBOid = null;
            }

            DestroyShaders();
        }

        /// <summary>
        /// Unproject support
        /// </summary>
        Vector3 screenToWorld ( int x, int y, float z = 0.0f )
        {
            Matrix4 modelViewMatrix, projectionMatrix;
            GL.GetFloat( GetPName.ModelviewMatrix, out modelViewMatrix );
            GL.GetFloat( GetPName.ProjectionMatrix, out projectionMatrix );

            return Geometry.UnProject( ref projectionMatrix, ref modelViewMatrix, glControl1.Width, glControl1.Height, x, glControl1.Height - y, z );
        }

        private void glControl1_MouseDown ( object sender, MouseEventArgs e )
        {
            // right mouse click        
            if ( !tb.MouseDown( e ) )
            {
                if ( checkAxes.Checked || triangleSelect || vertexSelect || ringSelect )
                {
                    // pointing to the scene:
                    pointOrigin = screenToWorld( e.X, e.Y, 0.0f );
                    pointTarget = screenToWorld( e.X, e.Y, 1.0f );
                    eye = tb.Eye;
                    pointDirty = true;

                    if ( selectedPoints == null ) return;
                    if ( (ModifierKeys & Keys.Control) == Keys.Control )
                    {
                        selectedPoints.Deselect = true;
                    }
                    else selectedPoints.Deselect = false;
                }
            }

        }

        private void glControl1_MouseUp ( object sender, MouseEventArgs e )
        {
            tb.MouseUp( e );
        }

        private void glControl1_MouseMove ( object sender, MouseEventArgs e )
        {
            tb.MouseMove( e );
        }

        private void glControl1_MouseWheel ( object sender, MouseEventArgs e )
        {
            tb.MouseWheel( e );
        }

        private void glControl1_KeyDown ( object sender, KeyEventArgs e )
        {
            tb.KeyDown( e );

            if (e.KeyCode == Keys.Delete)
            {
                buttonDeleteSelection_Click( sender, e );
            }
        }

        private void glControl1_KeyUp ( object sender, KeyEventArgs e )
        {
            if ( !tb.KeyUp( e ) )
                if ( e.KeyCode == Keys.F )
                {
                    e.Handled = true;
                    if ( frustumFrame.Count > 0 )
                        frustumFrame.Clear();
                    else
                    {
                        float N = 0.0f;
                        float F = 1.0f;
                        int R = glControl1.Width - 1;
                        int B = glControl1.Height - 1;
                        frustumFrame.Add( screenToWorld( 0, 0, N ) );
                        frustumFrame.Add( screenToWorld( R, 0, N ) );
                        frustumFrame.Add( screenToWorld( 0, B, N ) );
                        frustumFrame.Add( screenToWorld( R, B, N ) );
                        frustumFrame.Add( screenToWorld( 0, 0, F ) );
                        frustumFrame.Add( screenToWorld( R, 0, F ) );
                        frustumFrame.Add( screenToWorld( 0, B, F ) );
                        frustumFrame.Add( screenToWorld( R, B, F ) );
                    }
                }


        }

        private void buttonReset_Click ( object sender, EventArgs e )
        {
            tb.Reset();
        }

        private void buttonExportPly_Click ( object sender, EventArgs e )
        {
            if ( scene == null ||
                 scene.Triangles < 1 ) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file";
            sfd.Filter = "PLY Files|*.ply" + "|OBJ Files|*.obj";
            sfd.AddExtension = true;
            sfd.FileName = "";
            if ( sfd.ShowDialog() != DialogResult.OK )
                return;

            string ext = Path.GetExtension( sfd.FileName );

            switch ( ext )
            {
                case ".ply":
                    StanfordPly plyWriter = new StanfordPly();
                    plyWriter.TextFormat = true;
                    plyWriter.NativeNewLine = true;
                    plyWriter.Orientation = checkOrientation.Checked;
                    //plyWriter.DoNormals     = false;
                    //plyWriter.DoTxtCoords   = false;
                    plyWriter.DoColors = false;
                    using ( StreamWriter writer = new StreamWriter( new FileStream( sfd.FileName, FileMode.Create ) ) )
                    {
                        plyWriter.WriteBrep( writer, scene );
                    }
                    break;
                case ".obj":
                    WavefrontObj objWriter = new WavefrontObj();
                    using ( StreamWriter writer = new StreamWriter( new FileStream( sfd.FileName, FileMode.Create ) ) )
                    {
                        objWriter.WriteBrep( writer, scene );
                    }
                    break;

                default:
                    break;
            }


        }

        private void labelFile_MouseHover ( object sender, EventArgs e )
        {
            tt.Show( Util.TargetFramework + " (" + Util.RunningFramework + "), OpenTK " + Util.AssemblyVersion( typeof( Vector3 ) ),
                     (IWin32Window)sender, 10, -25, 4000 );
        }

        private void checkTriangles_CheckedChanged ( object sender, EventArgs e )
        {
            if ( triangleSelect == false )
            {
                triangleSelect = true;
                Cursor cur = Cursors.Cross;
                this.Cursor = cur;
            }
            else
            {
                triangleSelect = false;
                this.Cursor = Cursors.Default;
            }

            if ( vertexSelect )
                vertexSelect = !triangleSelect;

            if ( ringSelect )
                ringSelect = !triangleSelect;
        }

        private void checkVertices_CheckedChanged ( object sender, EventArgs e )
        {
            if ( vertexSelect == false )
            {
                vertexSelect = true;
                Cursor cur = Cursors.Cross;
                this.Cursor = cur;
            }
            else
            {
                vertexSelect = false;
                this.Cursor = Cursors.Default;
            }                       

            if ( triangleSelect )
                triangleSelect = !vertexSelect;

            if ( ringSelect )
                ringSelect = !vertexSelect;
        }

        private void buttonDeleteSelection_Click ( object sender, EventArgs e )
        {
          
        }

        private void undoButton_Click ( object sender, EventArgs e )
        {
            commandManager.Undo();
            if (commandManager.RedoCount > 0)
                redoToolStripMenuItem.Enabled = true;
            else redoToolStripMenuItem.Enabled = false;

            PrepareDataBuffers();

            if ( commandManager.UndoCount > 0 )
                undoToolStripMenuItem.Enabled = true;
            else undoToolStripMenuItem.Enabled = false;
        }

        private void button_RingSelection_CheckedChanged ( object sender, EventArgs e )
        {
            if ( ringSelect == false )
            {
                ringSelect = true;
                Cursor cur = new Cursor( Properties.Resources.brushBlack.Handle );
                this.Cursor = cur;
            }
            else
            {
                ringSelect = false;
                this.Cursor = Cursors.Default;
            }


            if ( vertexSelect == true )
                vertexSelect = !ringSelect;

            if ( triangleSelect == true )
                triangleSelect = !ringSelect;

        }

        private async void button_ExportSelection_Click ( object sender, EventArgs e )
        {

            if ( scene == null ||
                 scene.Triangles < 1 ) return;

            if ( selectedPoints.GetCount() == 0 )
            {
                MessageBox.Show( "No points on the source mesh are selected." );
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file";
            sfd.Filter = "PLY Files|*.ply" + "|OBJ Files|*.obj";
            sfd.AddExtension = true;
            sfd.FileName = "";
            if ( sfd.ShowDialog() != DialogResult.OK )
                return;

            string ext = Path.GetExtension( sfd.FileName );
            WavefrontObj objWriter = null;

            switch ( ext )
            {
                case ".ply":
                    StanfordPly plyWriter = new StanfordPly();
                    plyWriter.TextFormat = true;
                    plyWriter.NativeNewLine = true;
                    plyWriter.Orientation = checkOrientation.Checked;
                    plyWriter.DoColors = false;

                    handler = new WriteBrepHandler( plyWriter.WriteBrepSelection );

                    break;
                case ".obj":
                    objWriter = new WavefrontObj();
                    handler = new WriteBrepHandler( objWriter.WriteBrepSelection );
                    break;

                default:
                    break;
            }


            // lambda will be run on the UI thread
            var progress = new Progress<string>( ReportProgress );
            label_percentageReport.Text = "Saving 0%";

            // writing the file is run on the thread pool

            StreamWriter writer = null;
            stopButton.Visible = true;

            // get prepared triangle and vertex pointers
            List<int> trianglePointers = null;
            List<int> vertexPointers = null;

            try
            {
                cts = new CancellationTokenSource();
                writer = new StreamWriter( new FileStream( sfd.FileName, FileMode.Create ) );
                await Task.Run( () => handler( writer, scene, selectedPoints, progress, cts.Token, out trianglePointers, out vertexPointers ) );

            }
            catch ( OperationCanceledException )
            {

            }
            finally
            {
                RewriteReportLabel();
                cts = null;

                if ( writer != null )
                    writer.Close();
            }

            // support: Export batch
            String name = null;
            if ( sender is ToolStripMenuItem )
                name = (sender as ToolStripMenuItem).Name;
            if ( name == "exportBatch" )
            {
                cts = new CancellationTokenSource();
                ExportBatch eb = new ExportBatch( vertexPointers, trianglePointers, Path.GetDirectoryName( sfd.FileName ), ext );
                Task task = new Task ( () => eb.ProcessDirectory( progress, cts.Token, () => RewriteReportLabel() ) );
                task.RunSynchronously();
                stopButton.Visible = true;
            }

        }

        private void RewriteReportLabel()
        {
            if ( cts.IsCancellationRequested )
            {
                Action test = () =>
                {
                   this.toolStrip1.Invoke(
                     new MethodInvoker( () => this.label_percentageReport.Text = "File saving cancelled" ) );
                };

                //label_percentageReport.Text = "File saving cancelled";
            }
            else
            {
                Action writeReport = () =>
                {
                    this.toolStrip1.Invoke(
                     new MethodInvoker( () => this.label_percentageReport.Text = "File saved" ) );
                };
                //label_percentageReport.Text = "File saved";

                writeReport.Invoke();
            }

            // start timer
            Action startTimer = () =>
            {
                this.toolStrip1.Invoke(
                  new MethodInvoker( delegate ()
                  {
                      timer.Start();
                  } ) );
            };
            startTimer.Invoke();

            stopButton.Visible = false;
        }

        private void exportBatch_Click ( object sender, EventArgs e )
        {
            // prcess current file first 
            button_ExportSelection_Click( sender, e );

        }

        public void ReportProgress ( string report )
        {
            label_percentageReport.Text = report;
        }

        private void exportToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            buttonExportPly_Click( sender, e );
        }

        private void exportSelectionToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            button_ExportSelection_Click( sender, e );
        }

        private void loadSceneToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            buttonOpen_Click( sender, e );
        }

        private void loadTextureToolStripMenuItem_Click ( object sender, EventArgs e )
        {
            buttonLoadTexture_Click( sender, e );
        }

        private void brushSelection_CheckedChanged ( object sender, EventArgs e )
        {
            if ( brushSelection.Checked )
            {
                Cursor cur = new Cursor( Properties.Resources.brushBlack.Handle );
                this.Cursor = cur;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }

            if ( triangleSelection.Checked )
                triangleSelection.Checked = !brushSelection.Checked;

            if ( pointSelection.Checked )
                pointSelection.Checked = !brushSelection.Checked;
        }

        private void pointSelection_CheckedChanged ( object sender, EventArgs e )
        {
            if ( brushSelection.Checked )
                brushSelection.Checked = !pointSelection.Checked;

            if ( triangleSelection.Checked )
                triangleSelection.Checked = !pointSelection.Checked;


        }

        private void triangleSelection_CheckedChanged ( object sender, EventArgs e )
        {
            if ( brushSelection.Checked )
                brushSelection.Checked = !triangleSelection.Checked;

            if ( pointSelection.Checked )
                pointSelection.Checked = !triangleSelection.Checked;
        }


        private void stopButton_Click ( object sender, EventArgs e )
        {
            if ( cts != null )
            {
                cts.Cancel();
                label_percentageReport.Text = "";
                cts = null;
            }
        }

        private void toolStrip1_MouseEnter ( object sender, EventArgs e )
        {
            if ( savedCursor == null )
            {
                savedCursor = this.Cursor;
                this.Cursor = Cursors.Default;
            }

        }

        private void toolStrip1_MouseLeave ( object sender, EventArgs e )
        {
            this.Cursor = savedCursor;
            savedCursor = null;
        }

        private void newLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void startLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pointLogFileName = "log";
            pointLogFileName += pointLogNumber.ToString();
            pointLogFileName += ".txt";

            pointLogFile = new StreamWriter(pointLogFileName, false);

            stopLogToolStripMenuItem.Enabled = true;
            pointLogNumber++;
        }

        private void stopLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointLogFile.Close();
            stopLogToolStripMenuItem.Enabled = false;
            pointLogFile = null;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            commandManager.Redo();
            if (commandManager.UndoCount > 0)
                undoToolStripMenuItem.Enabled = true;
            else undoToolStripMenuItem.Enabled = false;

            PrepareDataBuffers();

            if (commandManager.RedoCount > 0)
                redoToolStripMenuItem.Enabled = true;
            else redoToolStripMenuItem.Enabled = false;
        }
    }

    public static class TaskExtension
    {
        public static Task ContinueWith ( this Task task, Action action )
        {
            return task.ContinueWith( _ => action() );
        }
    }





}
