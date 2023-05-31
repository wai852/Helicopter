using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Helicopter
{
    class Game:GameWindow
    {
        float eyePosX = 2;
        float eyePosY = 5;
        float eyePosZ = 2;

        float targetPosX = 0.0f;
        float targetPosY = 0.0f;
        float targetPosZ = 0.0f;

        const int coin_Nb = 10; //stage4 more flexible to change value
        const double coin_RADIUS = 0.5f;//stage4
        const float coin_Thickness = 0.2f;//stage4

        Coin[] coins = new Coin[10];//stage3
        int collectedCoinsCount = 0;//stage4

        Bitmap heightMap; //stage5
        //135 206 250 for stage 5 light blue sky
        const float rvalue = 135f / 255f;
        const float gvalue = 206f/ 255f;
        const float bvalue = 250f/ 255f;

        //int textureID;      //stage2
        int[] textureID = new int[3]; //stage4 load two texture + 1(additional tasks)
        float rotationAngle; //stage2
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {
            this.KeyDown += windowKeydown;
        }

        private void windowKeydown(object sender, KeyboardKeyEventArgs e) //stage3
        { //09-04
            float dX = targetPosX - eyePosX;
            float dY = targetPosY - eyePosY;
            float dZ = targetPosZ - eyePosZ; //res=is negative value
            float len = (float)Math.Sqrt(dX * dX + dY * dY);
            Random r = new Random();
            float angle = r.Next(0, 180);
            //able to move the camera around the helicopter
            switch (e.Key) {

                case Key.W:
                    eyePosX += dX * 0.01f * len;
                    eyePosY += dY * 0.01f * len;
                    targetPosX += dX * 0.01f * len;
                    targetPosY += dY * 0.01f * len; 
                    break;
                case Key.S:
                    eyePosX -= dX * 0.01f * len;
                    eyePosY -= dY * 0.01f * len;
                    targetPosX -= dX * 0.01f * len;
                    targetPosY -= dY * 0.01f * len; 
                    break;
                case Key.A:
                    eyePosX += dY * 0.05f;
                    eyePosY -= dX * 0.05f; 
                    break;
                case Key.D:
                    eyePosX -= dY * 0.05f;
                    eyePosY += dX * 0.05f; 
                    break;
                //case Key.Up: //camera zoom close
                //    eyePosX += dX * 0.01f;
                //    eyePosY += dY * 0.01f;
                //    eyePosZ += dZ * 0.01f;
                //    break;
                //case Key.Down: //camera zoom far
                //    eyePosX -= dX * 0.01f;
                //    eyePosY -= dY * 0.01f;
                //    eyePosZ -= dZ * 0.01f;
                //    break;

                //the result is of dz is negative
                //so eyePosz= eyeposz -(-dz)*0.01f, so moving up
                //Additional tasks
                case Key.Up: //camera zoom close
                     eyePosZ  -= dZ * 0.01f; //camera follow the view also
                     targetPosZ-= dZ * 0.01f;
                    break;
                case Key.Down: //camera zoom far
                    eyePosZ += dZ * 0.01f;
                    targetPosZ += dZ * 0.01f;
                    break;
                case Key.Escape: //Close the program
                    Close();
                    break;
                case Key.Q:
                    for (int i = 0; i < 5; i++) {

                        eyePosX += dX * 0.01f * angle;
                        eyePosY += dY * 0.01f * angle;
                        eyePosZ += dZ * 0.01f * angle;
                        targetPosX += dX * 0.01f;
                        targetPosY += dY * 0.01f;
                        targetPosZ += dZ * 0.01f;
                    }
                    break;
            }
        }
        void CheckCoinTouch()
        { //stage4 
            Vector3 spherePosition = new Vector3(targetPosX, targetPosY, targetPosZ);
            for (int i = 0; i < coin_Nb; i++) {
                if ((coins[i].center - spherePosition).Length < coins[i].radius + 1) {
                    Random R = new Random();
                    Vector3 position = new Vector3(R.Next(20) - 10, R.Next(20) - 10, R.Next(2));
                    Vector3 normal = new Vector3(R.Next(), R.Next(), 0);
                    coins[i] = new Coin(coin_RADIUS, coin_Thickness, position, normal);                  
                    collectedCoinsCount++;             
                }          
            }
        }
        void DrawBlades() { //for animate the propeller blades
            GL.Begin(PrimitiveType.Triangles); //use triangles

            GL.Color3(Color.Black); //the color of the blades
            //B1
            GL.Vertex3(1.5f * Math.Sin(rotationAngle + 0.2f), 1.5f * Math.Cos(rotationAngle + 0.2f), 1.0f);
            GL.Vertex3(0.0f,0.0f,1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotationAngle - 0.2f), 1.5f * Math.Cos(rotationAngle - 0.2f), 1.0f);
            //B2
            GL.Vertex3(1.5f * Math.Sin(rotationAngle + 0.2f + 3.14f /2.0f), 1.5f * Math.Cos(rotationAngle + 0.2f + 3.14f / 2.0f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotationAngle - 0.2f + 3.14f / 2.0f), 1.5f * Math.Cos(rotationAngle - 0.2f + 3.14f / 2.0f), 1.0f);
            //B3
            GL.Vertex3(1.5f * Math.Sin(rotationAngle + 0.2f +2  * 3.14f / 2.0f), 1.5f * Math.Cos(rotationAngle + 0.2f +2* 3.14f / 2.0f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotationAngle - 0.2f +2  * 3.14f / 2.0f), 1.5f * Math.Cos(rotationAngle - 0.2f +2* 3.14f / 2.0f), 1.0f);
            
            //B4
            GL.Vertex3(1.5f * Math.Sin(rotationAngle + 0.2f + 3 * 3.14f / 2.0f), 1.5f * Math.Cos(rotationAngle + 0.2f + 3 * 3.14f / 2.0f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotationAngle - 0.2f + 3 * 3.14f / 2.0f), 1.5f * Math.Cos(rotationAngle - 0.2f + 3 * 3.14f / 2.0f), 1.0f);
        }
        public void DrawGrid(Color color, float x, float y, float z, int cell_size = 1, int grid_size = 256) {
            //draw grid stage 2
            int dx = (int)Math.Round(x / cell_size) * cell_size;
            int dy = (int)Math.Round(y / cell_size) * cell_size;

            int stepsNumber = grid_size / cell_size;

            GL.PushMatrix();
            GL.Translate(dx - grid_size / 2, dy - grid_size / 2, 0);

            GL.Color3(color);
            GL.Begin(BeginMode.Lines); //use lines to draw the floor
            for (int i = 0; i < stepsNumber + 1; i++) {
                int currentPosition = i * cell_size;
                GL.Vertex3(currentPosition, 0, z);
                GL.Vertex3(currentPosition, grid_size, z);
                GL.Vertex3(0,currentPosition,z);
                GL.Vertex3(grid_size,currentPosition,z);
            }
            GL.End();
            GL.PopMatrix();
        }
        int loadTexture(string fileName) { //stage 2
            try
            {
                Bitmap image = new Bitmap(fileName); //assgin image by the filepath
                int texID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texID);
                BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                //tmpdata and then pass it to the GL.textImage2D
                                                                                                                       //blue,green,red,alpha
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                image.UnlockBits(data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                return texID;

            }
            catch (FileNotFoundException e) { return -1; }
        }
        
        void setupViewport() {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);

            int viewportWidth = this.Width;
            int viewportHeight = this.Height;

            Matrix4 projectionMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, viewportWidth / (float)viewportHeight, 
                1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMat);
        }
        //protected override void OnUpdateFrame(FrameEventArgs e)
        //{
        //    //Every physics change will be updated here
        //    KeyboardState input = Keyboard.GetType();
        //    if (input.IsKeyDown(Key.Escape)) { Exit();  }


        //    base.OnUpdateFrame(e);
        //}

        Color calculateColorfromHeight(int value) //stage5
        { //To calculate color depending on pixel intensity
            if (value < 50) return Color.FromArgb(50 + value, 50 + value, 90 + value * 2); //to blue
            else if (value < 100) return Color.FromArgb(value * 2 + 50, value * 2 + 30, value);
            else if (value > 200) return Color.FromArgb(255, 255, 255);//white,green and brown
            return Color.FromArgb(value / 3 * 2, value, value / 2);
        }

        public void drawLandScape(float X, float Y, float Z, int cell_size = 1) {
            int grid_size = 64;
            int stepX = heightMap.Width / grid_size;
            int stepY = heightMap.Height / grid_size;

            GL.PushMatrix();

            int ix, iy;
            double x, y, z;
            float r, g, b;
            for (iy = 0; iy < grid_size - 1; iy++) {
                GL.Begin(PrimitiveType.QuadStrip);
                for (ix = 0; ix < grid_size - 1; ix++) {
                    int mapValue = heightMap.GetPixel(ix * stepX, iy * stepY).R;
                    GL.Color3(calculateColorfromHeight(mapValue));
                    x = cell_size * (ix - grid_size / 2);
                    y = cell_size * (iy - grid_size / 2);
                    //z = 0; if dont change this value , our helicopter will on the ground
                    z = (double)mapValue / 32 + Z;
                    GL.Normal3(0, 0, 1);
                    GL.TexCoord2((float)(ix * stepX) / heightMap.Width, (float)(iy * stepY) / heightMap.Height);
                    GL.Vertex3(x, y, z);

                    mapValue = heightMap.GetPixel(ix * stepX, (iy + 1) * stepY).R;
                    GL.Color3(calculateColorfromHeight(mapValue));
                    x = cell_size * (ix - grid_size / 2);
                    y = cell_size*((iy+1)-grid_size/2);
                    //z = 0;  same for here
                    //the point height will be calculated depending on pixel intensity(bigger is higher)
                    z = (double)mapValue / 32 + Z;  
                    GL.Normal3(0, 0, 1);
                    GL.TexCoord2((float)(ix * stepX) / heightMap.Width, (float)((iy + 1) * stepY) / heightMap.Height);
                    GL.Vertex3(x, y, z);

                }
                GL.End();
            }
            GL.PopMatrix();
        }
        private void setupLightning() { //stage5
            //enables one light source; GL supports up to 8light sources by default
                                   //the first value higher look more brown(sunset)
            //float[] light_ambient = { 0.8f, 0.4f, 0.4f, 1.0f }; //the initial value for surroundings light
            float[] light_ambient = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light_diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] spotdirection = { 0.0f, 0.0f, -1.0f };
            GL.Light(LightName.Light0, LightParameter.Ambient, light_ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, light_diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, light_specular);

            GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, 1.8f);
            GL.Light(LightName.Light0, LightParameter.SpotCutoff, 45.0f);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, spotdirection);
            GL.Light(LightName.Light0, LightParameter.SpotExponent, 0.0f);

            GL.LightModel(LightModelParameter.LightModelLocalViewer,1.0f);
            GL.LightModel(LightModelParameter.LightModelTwoSide,1.0f);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ShadeModel(ShadingModel.Smooth);
        }
        protected override void OnLoad(EventArgs e)
        {
            heightMap = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "..\\Images\\heightmap.png");//stage5

            //change to array for each textureID
            textureID[0] = loadTexture(AppDomain.CurrentDomain.BaseDirectory + "..\\Images\\arrow.png");  
            textureID[1] = loadTexture(AppDomain.CurrentDomain.BaseDirectory + "..\\Images\\coinB.jpg");
            textureID[2] = loadTexture(AppDomain.CurrentDomain.BaseDirectory + "..\\Images\\coinF.png");
            //GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f); //default color for background 
            GL.ClearColor(rvalue, gvalue,bvalue, 1.0f);//135 206 250 stage5 change the color to light blue
            setupViewport();
            setupLightning();
            //stage3 09-04
            Random R = new Random();
            float angle = R.Next(0, 180);
            for (int i = 0; i < coin_Nb; i++) { //each time new position for Vertical
                Vector3 position = new Vector3(R.Next(20) - 10, R.Next(20) - 10, R.Next(2));
                Vector3 normal = new Vector3(R.Next(), R.Next(), 0);
                coins[i] = new Coin(coin_RADIUS, coin_Thickness, position , normal);
               
            }
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {  //each frame
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            rotationAngle += 0.1f; //The position of the blades depends on the
                                   //rotationAngle variable, need to continuously change its value
                                   //so each frame give this R.Angle update 
            CheckCoinTouch(); //stage4
            Render();
            this.Title = "Nb of collected Coins: " + collectedCoinsCount.ToString();//show the counter in title

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
        void Render() {
            //This martrix Control(move) the Camera in a virtual 3D scene
            Matrix4 modelviewMat = Matrix4.LookAt(eyePosX, eyePosY, eyePosZ, targetPosX, targetPosY, targetPosZ, 0f, 0f, 1f);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelviewMat);

            ////Draw floor-09-04 //stage3
            //DrawGrid(Color.Green, 0, 0, -1, -1, 256); //we will have our own floor with picture so no need in Stage5

            drawLandScape(0, 0, -7, 2); //stage5

            //Draw coins //stage3
            GL.Color3(Color.Yellow);
            GL.Enable(EnableCap.Texture2D);//stage4 :Enable texturing of coins in Render function
            GL.BindTexture(TextureTarget.Texture2D, textureID[1]);//use our coin picture which store in array[1]
            //GL.BindTexture(TextureTarget.Texture2D, textureID[2]);
            for (int i = 0; i < coin_Nb; i++)
                drawCylinder(coins[i].radius,coins[i].width,coins[i].center,coins[i].normal,20, 1);
            //it will be near the start point
            GL.Disable(EnableCap.Texture2D);


            //Move the helicopter 09-04 //stage3
            GL.Translate(targetPosX, targetPosY, targetPosZ);

            /* Old Version
            //Draw propeller Blades
            // GL.Begin(PrimitiveType.Quads);

            // GL.Color3(0f, 0f, 0f);
            // GL.Vertex3(-1.0f, -0.1f, 1.0f);
            // //GL.Color3(0f, 1f, 0f);
            // GL.Vertex3(-1.0f, 0.1f, 1.0f);
            // //GL.Color3(1f, 0f, 0f);
            // GL.Vertex3(1.0f, 0.1f, 1.0f);
            //// GL.Color3(0f, 0f, 1f);
            // GL.Vertex3(1.0f, -0.1f, 1.0f);

            // GL.Color3(0f, 0f, 0f);
            // GL.Vertex3(-0.1f, -1.0f, 1.0f);
            // GL.Vertex3(0.1f, -1.0f, 1.0f);
            // GL.Vertex3(0.1f, 1.0f, 1.0f);
            // GL.Vertex3(-0.1f, 1.0f, 1.0f);
            // //Draw more Blades
            */

            //New version:
            DrawBlades();

            GL.End();
            //Draw Sphere
            //GL.Color3(0f, 0f, 1f);//Set The Color of Sphere
            GL.Color3(Color.Yellow);    //stage2
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureID[0]);//stage4:change the textureID to array for the picture of sphere
            
            sphere(1.0, 20, 20);
            GL.Disable(EnableCap.Texture2D);//stage2
            //DrawGrid(Color.AliceBlue, 0, 0, -1, 1, 256);//stage2, need to delete it when we are in Stage5
        }
        void sphere(double r, int nx, int ny) {  //draw a sphere
            int i, ix, iy;
            double x, y, z;
            for (iy = 0; iy < ny; iy++) {

                GL.Begin(BeginMode.QuadStrip); //can set 2 points
                for (ix = 0; ix <= nx; ix++) {  
                    //From Buttom
                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos(iy * Math.PI / ny);
                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos((iy + 1) * Math.PI / ny);
                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)(iy+1) / (double)ny);
                    GL.Vertex3(x, y, z);

                }
                GL.End();
            }
        }
        void drawCylinder(double radius, float width, Vector3 center, Vector3 normal, int nx, int ny)
        { //stage3
            int ix, iy;
            double x, y, z;
            GL.Translate(center);
            Random r = new Random();
            float angle=r.Next(0, 180);
            if (normal != Vector3.UnitZ) {
                normal.Normalize(); // Math.Acos(normal.Z)
                GL.Rotate(57.2957795 * Math.Acos(normal.Z), new Vector3d(normal.Y, -normal.X, 0));
                //GL.Rotate(angle, new Vector3d(normal.Y, -normal.X, 0));
            }
            for (iy = 0; iy < ny; iy++) {
                GL.Begin(BeginMode.QuadStrip);
                for (ix = 0; ix <= nx; ix++) {
                    x = radius * Math.Cos(2 * ix * Math.PI / nx);
                    y = radius * Math.Sin(2 * ix * Math.PI / nx);
                    z = iy / ny * width;
                    GL.Normal3(x, y, 0);
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);

                    x = radius * Math.Cos(2 * ix * Math.PI / nx);
                    y = radius * Math.Sin(2 * ix * Math.PI / nx);
                    z = (iy + 1) / ny * width;
                    GL.Normal3(x, y, 0);
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);
                }
                GL.End();
            }
            for (int i = 0; i < 2; i++) {
                GL.BindTexture(TextureTarget.Texture2D, textureID[i + 1]);
                GL.Begin(BeginMode.TriangleFan);
                GL.TexCoord2(0.5f, 0.5f); // stage 4: for creating texture coordinates in coin triangles
                GL.Vertex3(0, 0, i * width);
                for (ix = 0; ix <= nx; ix++) {
                    x = radius * Math.Cos(2 * ix * Math.PI / nx);
                    y = radius * Math.Sin(2 * ix * Math.PI / nx);
                    double s = (x + radius) / (2 * radius); // stage 4: for creating texture coordinates in coin triangles
                    double t = (y + radius) / (2 * radius); // stage 4: for creating texture coordinates in coin triangles
                    GL.TexCoord2(t, s);
                    GL.Vertex3(x, y, i * width);
                }
                GL.End();
            }
            if (normal != Vector3.UnitZ) 
                GL.Rotate(-57.2957795 * Math.Acos(normal.Z), new Vector3d(normal.Y, -normal.X, 0));
                //GL.Rotate(-angle, new Vector3d(normal.Y, -normal.X, 0));
            GL.Translate(-center);      
        }
    }
    public class Coin
    {//stage3
        public double radius;
        public float width;
        public Vector3 center;
        public Vector3 normal;
        public bool touched;

        public Coin(double newR, float newW, Vector3 newC, Vector3 newN) {
            this.radius = newR;
            this.width = newW;
            this.center = newC;
            this.normal = newN;
            this.touched = false;
        }

    }
}
