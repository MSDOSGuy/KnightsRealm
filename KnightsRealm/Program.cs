using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Drawing;
using System.Diagnostics;
using OpenTK.Platform.Windows;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Xml;
using System.Reflection;
using System.ComponentModel.Design;
using OpenTK.Input;
using System.Runtime.InteropServices.ComTypes;

// PROTECTED UNDER THE MIT LICENSE 
// MADE BY: MIKE FERNANDEZ
// STARTED IN 5/30/20

// update: tons of redundant code, an utter piece of shit. Only intro is made and in fucking immediate mode/fixed function pipeline with an evident
// obscure knowledge on programming and terrible negligent mistakes. 

namespace ConsoleApp2
{
    public class game
    {
        //-------------------------------------------------------------------------------------------------------------------------------------
        // Purpose: creates some variables that are specific to the window creation, and display attributes such as framerate, resolution etc...
        // stored in arrays that will be changed later if necessary by the launcher for user desired variations in those particular display variables
        //-------------------------------------------------------------------------------------------------------------------------------------    
        // doesn't make sense above, too lazy to fix all the outdated comments, don't care because this code is shit and doesn't do anything particuarly fancy 
        public double x = 25;
        public double y = 25;
        public double z = -20;
        static public double angle = -0.1;
        static public bool resolution_error; 
        public static int result = 0;
        static double[] XYMenuCoords = { 20, 20 };
        public static int swrender = 0;
        public static decimal programtime = 0;
        public static int programmin = 0;
        public static bool endrendersecondscene_3;
        public static bool tex1idinit = false;
        public static bool tex2idinit = false;
        public static string savefileaddress = "SavedGameFile.xml";
        public static int writedatacounter;
        public static string slplaceholder;
        public static bool debug = true;
        public static bool frameratehasbeenmanipulated;
        // this var saves any resolution a user has made in the launcher so next launch it is the same, defaults to 0,0,0 if the user
        // hasn't changed it. Code detects if it is 0,0,0 ( default value ) if so, it just defaults to the normal resolution of 1200, 900 @60fps if left untouched
        public static int[] displaypropertiesloadedXMLvalues = { 0, 0, 0 };
        public static int[] displayproperties = { 1200, 900, 60 }; // Display Resolution, and other variables concerning framerates etc for the game...
        public static int[] displaypropertiesdefault = { 1200, 900, 60 };
        public static int[] currentviewport = { 0, 0, 1200, 900 }; // current viewport for any rendered models, landscapes etc...      
        public static double[] Orthoresovalues = { displayproperties[0], displayproperties[1] };
        public static double[] Orthovalues = { 0.0, Orthoresovalues[0] + .0, 0.0, Orthoresovalues[1] + .0, -1.0, 1.0 };
        public static float[] ClearColorvalues = { 0.0f, 0.0f, 0.0f, 0.0f };
        //---------------------------------------------------------------------------------------------
        // Purpose: part of the sound engine - creates some vars so music can be played, these vars will eventually all be combined to form a command to play a vbs file that
        // in return plays a mp3 file. This is done to make sure the respective mp3 file wmp is invisible when playing
        //---------------------------------------------------------------------------------------------
        //ps1
        public static string scriptName1 = "Assets\\Sounds\\sound1.vbs"; // full path to script
        public static int abc1 = 2;
        public static string name1 = "sound1.vbs";
        public static string nologo1 = "//nologo";
        public static ProcessStartInfo ps1 = new ProcessStartInfo();

        // ps2
        public static string scriptName2 = "Assets\\Sounds\\selectsound.vbs"; // full path to script
        public static int abc2 = 2;
        public static string name2 = "selectsound.vbs";
        public static ProcessStartInfo ps2 = new ProcessStartInfo();

        // ps3
        public static string scriptName3 = "Assets\\Sounds\\selectsound.vbs"; // full path to script
        public static int abc3 = 2;
        public static string name3 = "selectsound.vbs";

        public static ProcessStartInfo ps3 = new ProcessStartInfo();

        // SAVE AND LOAD ENGINE
        //===================== 
        // Structure goes as follows after creating new saveload class...
        // SaveLoadConfiguration.getNextFileName(savefileaddress);  ( will find a file name with a incremented value if it already exists )
        // change height, width, frameratecap, cap, and other game variables here
        // SaveLoadConfiguration.WritesGameData(savefileaddress); ( saves changed variables in the unique filename, new save files are created for redunancy )
        [System.Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
        public partial class SaveLoadConfiguration
        {

            public int height { get; set; }

            public int width { get; set; }

            public int frameratecap { get; set; }

            public bool cap { get; set; }

            public void WritesGameData(string fileName) // SavedGameData.xml
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var XML = new XmlSerializer(typeof(SaveLoadConfiguration));
                    XML.Serialize(stream, this);
                }

            }

            public SaveLoadConfiguration LoadGameData(string fileName) // SavedGameData.xml
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveLoadConfiguration));
                using (StreamReader LoadGameData = new StreamReader(fileName))
                {
                    return (SaveLoadConfiguration)serializer.Deserialize(LoadGameData);
                }

            }

            public void SaveAllChangedAsFinal()
            {
                displaypropertiesloadedXMLvalues[0] = height;
                displaypropertiesloadedXMLvalues[1] = width;
                displaypropertiesloadedXMLvalues[2] = frameratecap;
            }

        }
        public class ConsoleManipulation 
        {
            //------------------------------------------------------------
            // Purpose: allows code to hide console window or show it
            //------------------------------------------------------------
            // imports system libraries for console manipulation ( in this case to hide or show the window )
            [DllImport("kernel32.dll")]
            static public extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static public extern bool AnimateWindow(IntPtr hWnd, int nCmdShow);
            // sets hide and show variables and are shown ( or hidden ) by doing 
            public IntPtr handle = GetConsoleWindow();
            // ShowConsole(handle, SW_HIDE); ( replace SW_HIDE w/ SW_SHOW if your showing it, this would mainly be done for debugging if necessary )           
            public const int AW_HOR_POSITIVE = 0x00000001,
            AW_HOR_NEGATIVE = 0x00000002,
            AW_VER_POSITIVE = 0x00000004,
            AW_VER_NEGATIVE = 0x00000008,
            AW_CENTER = 0x00000010,
            AW_HIDE = 0x00010000,
            AW_ACTIVATE = 0x00020000,
            AW_SLIDE = 0x00040000,
            AW_BLEND = 0x00080000;            
            void AW(int a_function) // default console window selection, pass in animation through "a_function" ( e.g AW_HIDE ) 
            {
                AnimateWindow(handle, a_function); 
            }
            void AW(IntPtr handle, int a_function) // allows user to choose which window given a parameterized handle through overloading
            {
                AnimateWindow(handle, a_function);
            }
        }
        public static void Main(string[] args)
        {
            ConsoleManipulation HW_0 = new ConsoleManipulation(); 
            
            // 1/2 Sound Engine, Sound Files are coded in VBScript 
            currentviewport[2] = displayproperties[0];
            currentviewport[3] = displayproperties[1];
            MathHelper.GetFrameDurationFromRate(displayproperties[2]);

            // initilizes sound engine 
            // ps1
            ps1.FileName = "cscript.exe";
            ps1.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", nologo1, scriptName1, abc1, name1, null);
            //This will equate to running via the command line:
            // > cscript.exe "C:\\Program Files\\KnightsRealm C#\\Assets\\Sounds\\sound1.vbs" "2" "sound1.vbs"

            // ps2
            ps2.FileName = "cscript.exe";
            ps2.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", scriptName2, abc2, name2, null);

            // ps3
            ps3.FileName = "cscript.exe";
            ps3.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", scriptName3, abc3, name3, null);

            // stores sound vars into an array for ease of access for later use 
            ProcessStartInfo[] sounds = { ps1, ps2, ps3 };
            Intro();

        }
        static GameWindow Intro()
        {
            Window window2 = new Window(displayproperties[0], displayproperties[1]);      
            bool continueon = false;
            window2.Run(displayproperties[2]);
            SaveLoadConfiguration SLdata = new SaveLoadConfiguration();
            if (continueon == true)
            {
                // if user desires to save settings, otherwise goto end launcher 
                string savefileaddress = "SavedGameData.xml";
                SLdata.WritesGameData(savefileaddress);
                SLdata.SaveAllChangedAsFinal();
            }
            while (continueon == true)
            {
                if (displaypropertiesloadedXMLvalues[0] >= 680 && displaypropertiesloadedXMLvalues[1] >= 400)
                {
                    displayproperties[0] = displaypropertiesloadedXMLvalues[0];
                    displayproperties[0] = displaypropertiesloadedXMLvalues[0];
                }
                else
                {
                    displayproperties[0] = displaypropertiesdefault[0];
                    displayproperties[1] = displaypropertiesdefault[1];
                    goto checkforunder;
                }
                if (displaypropertiesloadedXMLvalues[2] > 0 && SLdata.cap == true)
                {
                    if (displaypropertiesloadedXMLvalues[2] < 30)
                    {
                        goto checkforzero; 
                    }
                    displayproperties[2] = displaypropertiesloadedXMLvalues[2];
                }
                else { if (displaypropertiesloadedXMLvalues[2] == 0) { goto checkforzero; } else if (SLdata.cap == false) { goto checkfornocap; } }
                displayproperties[2] = displaypropertiesloadedXMLvalues[2];
                window2.Width = displayproperties[0];
                window2.Height = displayproperties[1];
               
                goto continuefinal;
            checkforunder:
                // throw invalid resolution error and return to original menu & show console window
                resolution_error = true; 
                // goto original menu maybe by changing current frame iteration from programtime to reload the menu? 
            checkforzero:;
                // throw invalid framerate error & show console window
                Window.incorrect_framerate_error = true;

            checkfornocap:;
                // cap must be true if you desire to manipulate framerate lock
                Window.cap_error = true; 
                // throw error and show window
                // goto original menu maybe by changing current frame iteration from programtime to reload the menu? 
            }
        continuefinal:
            window2.Run(displayproperties[2]);
            return window2;
        }
        static void renderf2_5()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1f, 1f, 1f, 0.5f); // temporary transparent color to show depth 
            GL.Vertex3(100, 400, 1);
            GL.Vertex3(1100, 400, 1);
            GL.Vertex3(1100, 300, 1);
            GL.Vertex3(100, 300, 1);
            GL.End();
        }
        static void renderf2_4()
        {
            GL.Rotate(-0.1, 1, 1, 1);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.Vertex2(100, 600);
            GL.Vertex2(1100, 600);
            GL.Vertex2(1100, 800);
            GL.Vertex2(100, 800);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.Vertex3(400, 500, 0);
            GL.Vertex3(800, 500, 0);
            GL.Vertex3(800, 200, 0);
            GL.Vertex3(400, 200, 0);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1f, 1f, 1f, 0.5f); // temporary transparent color to show depth 
            GL.Vertex3(100, 400, 1);
            GL.Vertex3(1100, 400, 1);
            GL.Vertex3(1100, 300, 1);
            GL.Vertex3(100, 300, 1);
            GL.End();
        }
        static void renderf2_3()
        {
            GL.Rotate(0.1, 25, 25, -20);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1f, 1f, 1f, 0.5f); // temporary transparent color to show depth 
            GL.Vertex3(100, 400, 1);
            GL.Vertex3(1100, 400, 1);
            GL.Vertex3(1100, 300, 1);
            GL.Vertex3(100, 300, 1);
            GL.End();
        }
        static void renderf2_2()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1f, 1f, 1f, 0.5f); // temporary transparent color to show depth 
            GL.Vertex3(100, 400, 1);
            GL.Vertex3(1100, 400, 1);
            GL.Vertex3(1100, 300, 1);
            GL.Vertex3(100, 300, 1);
            GL.End();
        }

        static void renderf2()
        {

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Rotate(angle, 25, 25, -20); // after rotation, perhaps render a couple particles with point primitives in a single vertex buffer object 
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.Vertex2(100, 600); // 0,0     
            GL.Vertex2(1100, 600); // right most ( 1000 ), 0    
            GL.Vertex2(1100, 800); // right most ( 1000 ), top most ( 200 )      
            GL.Vertex2(100, 800); // 0, top most 
            GL.End();
            GL.Rotate(0.1, 50, 50, 2); // after rotation, perhaps render a couple particles effects with point primitives in a single vertex buffer object 
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.Vertex3(400, 500, 0);
            GL.Vertex3(800, 500, 0);
            GL.Vertex3(800, 200, 0);
            GL.Vertex3(400, 200, 0);
            GL.End();
        }
        static void renderf()
        {

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.Vertex2(100, 600);
            GL.Vertex2(1100, 600);
            GL.Vertex2(1100, 800);
            GL.Vertex2(100, 800);
            GL.End();
        }
        static void renderf_1()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.Vertex3(400, 500, 0);
            GL.Vertex3(800, 500, 0);
            GL.Vertex3(800, 200, 0);
            GL.Vertex3(400, 200, 0);
            GL.End();
        }

        static void renderf_2()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1f, 1f, 1f, 0.5f); // temporary transparent color to show depth 
            GL.Vertex3(100, 400, 1);
            GL.Vertex3(1100, 400, 1);
            GL.Vertex3(1100, 300, 1);
            GL.Vertex3(100, 300, 1);
            GL.End();

        }
        static void Keyboard_Input_Menu()
        {
            KeyboardState keyStateW = Keyboard.GetState();
            if (keyStateW.IsKeyDown(Key.W) && keyStateW.IsKeyUp(Key.W))
            {
                XYMenuCoords[1] = XYMenuCoords[1] + 20;
            }
            KeyboardState keyStateS = Keyboard.GetState();
            if (keyStateS.IsKeyDown(Key.S) && keyStateS.IsKeyUp(Key.S))
            {
                XYMenuCoords[0] = XYMenuCoords[0] - 20;
            }
            KeyboardState UpArrow = Keyboard.GetState();
            if (keyStateW.IsKeyDown(Key.Up) && keyStateW.IsKeyUp(Key.Up))
            {
                XYMenuCoords[1] = XYMenuCoords[1] + 20;
            }
            KeyboardState DownArrow = Keyboard.GetState();
            if (keyStateS.IsKeyDown(Key.Down) && keyStateS.IsKeyUp(Key.Down))
            {
                XYMenuCoords[0] = XYMenuCoords[0] - 20;
            }

        }
        static void title_s_KeyDown()
        {
            // if user is holding key, it will steadily accelerate on the menu selector
            KeyboardState keystate_hold_down = Keyboard.GetState();
            if (keystate_hold_down.IsKeyDown(Key.W))
            {
                XYMenuCoords[0] = XYMenuCoords[0] - 10;
                Thread.Sleep(650);
                XYMenuCoords[0] = XYMenuCoords[0] - 10;
            }
        }
        static void title_s_KeyUp()
        {
            // if user is holding key, it will steadily accelerate on the menu selector
            KeyboardState keystate_hold_up = Keyboard.GetState();
            if (keystate_hold_up.IsKeyDown(Key.S))
            {
                XYMenuCoords[0] = XYMenuCoords[0] + 10;
                Thread.Sleep(650);
                XYMenuCoords[0] = XYMenuCoords[0] + 10;
            }
        }
        class Texture1
        {
            int output_path_introtex;
            int tex1id = 0; // 0 serves as a null placeholder
            public Texture1(int input_path_introtex)
            {
                output_path_introtex = input_path_introtex;
            }
            public void SetupTextureID_Texture1()
            {
                tex1id = GL.GenTexture(); // generates texture id and stores it in handle var "tex1id" for later consumption                           
            }
            public void UseTex()
            {
                GL.BindTexture(TextureTarget.Texture2D, tex1id);
            }
        }

        class Window : GameWindow
        {
            bool endrendertransparentcordobject = false;
            static public bool incorrect_matrix_error;
            static public bool incorrect_framerate_error;
            static public bool cap_error;
            float[] introtexturecoords =
            {
                 100.0f,  600.0f, // lower-left corner  
                 1100.0f, 600.0f, // lower-right corner
                 1100.0f, 800.0f, // top-right corner
                 100.0f,  800.0f  // top-left corner
            };
            bool rendersecondscene = false;
            bool endrendersecondscene = false;
            bool endrendersecondscene_2 = false;         
            bool menu_loaded = false;
            double[] XYMenuCoords = { 20, 20 };
            public Window(int width, int height) : base(width, height) { }
            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                swrender++;
                float[] currenttranslationmatrix = { 0 };
                GL.GetFloat(GetPName.ModelviewMatrix, currenttranslationmatrix);
                if (debug)
                {
                    Console.Clear();
                    Console.WriteLine("Current Matrix:");
                    Console.WriteLine(currenttranslationmatrix[0]);
                    Console.WriteLine("Current Frame:");
                    Console.WriteLine(swrender);
                    Console.WriteLine("Current Program Time:");
                }
                bool isD = MathHelper.IsMultiple(swrender, displayproperties[2]); // checks if framecount is divisible by framerate, if so, then we can calculate programtime
                if (isD == true)
                {
                    programtime++;
                }
                if (debug)
                {
                    Console.WriteLine(programtime + " Secs"); // frame counter ( 360 frames = start of animation, 520 frames end of second animation ) 
                    Console.WriteLine("Rotation Axis");
                    Console.WriteLine(angle); // find what method var this variable passes to ( somewhere in GL.Rotate method ) then with a this keyword update frame with exact position hopefully
                }
                     string errorcheckresult = "No Errors Found!";          
                    if (incorrect_matrix_error)
                    {
                        errorcheckresult = "Error Found! 'incorrect_matrix_error'";
                    }
                    if (resolution_error)
                    {
                        errorcheckresult = "Error Found! 'resolution_error'";
                    }
                    if (incorrect_framerate_error)
                    {
                        errorcheckresult = "Error Found! 'incorrect_framerate_error'" + " , your framerate :" +displayproperties[2] + " is invalid!";
                    }
                    if (cap_error)
                    {
                    errorcheckresult = "Error Found! 'cap_error' You cannot select a framerate if framerate cap is off!";
                    }
                  
                Console.WriteLine("Status: " + errorcheckresult);
                Console.WriteLine("--------");
                if (incorrect_matrix_error)
                {
                    Console.Write(Environment.NewLine);
                    Console.WriteLine( programtime + " : " + "Matrix to be transposed is incorrect! Not enough vertexes to qualify as a proper primitive. Report Bug w/ screenshot of error!");
                }
                if (resolution_error)
                {
                    Console.Write(Environment.NewLine);
                    Console.WriteLine(programtime + " : " + "Error Found! 'resolution_error' in settings! If problem persists, reinstall and PLEASE report bug w/ screenshot of error!");               
                }
                if (incorrect_framerate_error)
                {
                    Console.Write(Environment.NewLine);
                    Console.WriteLine(programtime + " : " + "Error Found! 'incorrect_framerate_error'" + " , your framerate :" + displayproperties[2] + " is invalid! If problem persists, reinstall and PLEASE report bug w/ screenshot of error!");
                }
                if (cap_error)
                {
                    Console.Write(Environment.NewLine);
                    Console.WriteLine(programtime + " : " + "Error Found! 'cap_error' You cannot select a framerate if framerate cap is off! If problem persists, reinstall and PLEASE report bug w/ screenshot of error!");
                }
                Console.Write("Game Console, Do Stuff Here, Type Help for Commands >");
                switch (programtime)
                {
                    case 6: // counted in seconds for now, to make things more percise a way to count in ms ( despite the given framerate ) must be implemented someway...
                        rendersecondscene = true;
                        break;
                    case 8:
                        endrendersecondscene = true;
                        break;
                    case 10:
                        endrendersecondscene_2 = true;
                        break;
                    case 13:
                        endrendertransparentcordobject = true;
                        break;
                    case 17:
                        endrendersecondscene_3 = true;
                        break;
                    case 21:
                        // revert rotation matrix 
                        break;

                }
                if (menu_loaded || programtime >= 10) // menu_loaded is set to true when everything is said and done however after 10s~ if something goes wrong it will load anyway
                {
                    game.Keyboard_Input_Menu();
                    game.title_s_KeyDown();
                    game.title_s_KeyUp();
                }
                base.OnUpdateFrame(e);

            }
            protected override void OnResize(EventArgs e)
            {


                GL.Viewport(currentviewport[0], currentviewport[1], currentviewport[2], currentviewport[3]);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(Orthovalues[0], Orthovalues[1], Orthovalues[2], Orthovalues[3], Orthovalues[4], Orthovalues[5]);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                base.OnResize(e);
            }
            protected override void OnRenderFrame(FrameEventArgs e)
            {

                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.ClearColor(ClearColorvalues[0], ClearColorvalues[1], ClearColorvalues[2], ClearColorvalues[3]); // background color black
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                renderf();
                renderf_1();
                renderf_2();
                if (rendersecondscene)
                {
                    renderf2(); // disolve all shapes besides transparent rectangle
                    renderf2_2(); // transparent rectangle stays still but is affected by rotation matrix ( negate matrix somehow? )
                    
                }
                if (endrendersecondscene)
                {
                    renderf2_3(); // disolve transparent rectangle 
                }
                if (endrendersecondscene_2)
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit); // clear color data to clear screen      
                    renderf2_4(); // final model of game logo ( transisiton to title screen afterwards )                    
                }
                if (endrendersecondscene_3)
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                }
                if (endrendertransparentcordobject)
                {
                    renderf2_5();
                }
                Texture1 introtexture = new Texture1(8);
                introtexture.SetupTextureID_Texture1();

                SwapBuffers();
                // display intro 
                menu_loaded = true;
                base.OnRenderFrame(e);
            }

        }
        public class MathHelper
        {
            static public bool IsMultiple(int x, int n)
            {
                return (x % n) == 0;
            }
            static public int GetFrameDurationFromRate(int n)
            {
                result = 1000 / n;
                return result;
            }
            static public double[] TransposeMatrix(double[] matrix)
            {
                int a = 0;
                double[] returnmatrix = { 0 }; 
                // I'm almost certain there is a better way to check how many index positions are in an array, surely there is a better way...
                try
                {
                    if (matrix[8] >= a || matrix[8] < a) // just checking if it exists, couldn't care what it is equal to. If so, then this is a rectangle/square matrix, otherwise it is a triangle primitive
                    {
                        // transpose
                        return returnmatrix; 
                    }
                    else if (matrix[6] >= a || matrix[6] < a)
                    {
                        // transpose
                        return returnmatrix; 
                    }
                }
                catch
                {
                    Window.incorrect_matrix_error = true; 
                    // show console window
                }
                return returnmatrix;
            }
            static public double GetMagnitudeVector(double initialpoint_x, double initialpoint_y, double x, double y)
            {
                double magnitude;
                double sqrt; 
                sqrt = x - initialpoint_x * x + initialpoint_x + y - initialpoint_y * y + initialpoint_y; // square root of x - x2 squared plus y equivalent stored into a variable and returned 
                magnitude = Math.Sqrt(sqrt); // use distance formula aka pythagorean theorem 
                return magnitude;  // angle/magnitude is returned
            }
        }
    }
}




