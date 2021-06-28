using System.Diagnostics;
using INFOGR2019Tmpl8;
using OpenTK;
using System;
using OpenTK.Graphics.OpenGL;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
		static public Mesh mesh, floor, cube, cloud, boom, kop, table, kop2;            // a mesh to draw using OpenGL
		const float PI = 3.1415926535f;         // PI
		float a = 0;                            // teapot rotation angle
		Stopwatch timer;                        // timer for measuring frame duration
		Shader shader;                          // shader to use for rendering
		Shader postproc;                        // shader to use for post processing
		Texture wood, ground, purple, wolkig, gold, silver;                           // texture to use for rendering
		RenderTarget target;                    // intermediate render target
		ScreenQuad quad;                        // screen filling quad for post processing
		bool useRenderTarget = true;
		SceneGraph scene;

		// initialize
		public void Init()
		{
			scene = new SceneGraph(screen);
			// load teapot
			mesh = new Mesh( "../../assets/teapot.obj" );
			floor = new Mesh( "../../assets/floor.obj" );
			cube = new Mesh("../../assets/cube.obj");
			cloud = new Mesh("../../assets/cumulus00.obj");
			boom = new Mesh("../../assets/AL05m");
			kop = new Mesh("../../assets/cup.obj");
			table = new Mesh("../../assets/table.obj");
			kop2 = new Mesh("../../assets/cup.obj");
			// initialize stopwatch
			timer = new Stopwatch();
			timer.Reset();
			timer.Start();
			// create shaders
			shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
			postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
			// load a texture
			wood = new Texture( "../../assets/wood.jpg" );
			ground = new Texture("../../assets/ground.jpg");
			purple = new Texture("../../assets/purple.jpg");
			wolkig = new Texture("../../assets/cloud.jpg");
			gold = new Texture("../../assets/gold.jpg");
			silver = new Texture("../../assets/silver.jpg");
			// create the render target
			target = new RenderTarget( screen.width, screen.height );
			quad = new ScreenQuad();
			// create the hierachy
			SceneGraph.hierachy.Add(mesh, table);
			SceneGraph.hierachy.Add(kop, table);
			SceneGraph.hierachy.Add(kop2, table);
			SceneGraph.hierachy.Add(table, floor);

		}

		// tick for background surface
		public void Tick()
		{
			screen.Clear( 0 );
			screen.Print("hello world", 2, 2, 0xffff00);
		}

		// tick for OpenGL rendering code
		public void RenderGL()
		{
			//when 'l' is pressed on the keyboard a second lightsource will appear
			int lightning = 0;
			if (LightOn() == true)
			{
				lightning = 1;
			}
			if (LightOn() == false)
			{
				lightning = 0;
			}
			GL.ProgramUniform1(shader.programID, shader.uniform_lighto, lightning);

			// measure frame duration
			float frameDuration = timer.ElapsedMilliseconds;
			timer.Reset();
			timer.Start();

			// prepare matrix for vertex shader
			float angle90degrees = PI / 2;
			mesh.view = Matrix4.CreateScale(15.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), PI/2) * Matrix4.CreateTranslation(new Vector3(200 + (float)(5 * Math.Cos(a)), 350.0f, 200 - (float)(5 * Math.Sin(a))));	//Camera transformation.
			floor.view = Matrix4.CreateScale( 18.0f ) * Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), 0 );
			cube.view = Matrix4.CreateScale(2/4f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
			cloud.view = Matrix4.CreateScale(1.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
			boom.view = Matrix4.CreateScale(1.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
			kop.view = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0) * Matrix4.CreateTranslation(new Vector3(50+ (float)(5 * Math.Cos(a)), 350.0f, 100 - (float)(5 * Math.Sin(a))));
			kop2.view = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0) * Matrix4.CreateTranslation(new Vector3(-70 + (float)(5 * Math.Cos(a)), 350.0f, 100 + (float)(5 * Math.Sin(a))));
			table.view = Matrix4.CreateScale(1.0f / 108.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a) * Matrix4.CreateTranslation(new Vector3(0,-2,0));
			//	Matrix4 Tcamera = Matrix4.CreateTranslation( new Vector3( 0, -14.5f, 0 ) ) * Matrix4.CreateFromAxisAngle( new Vector3( 1, 0, 0 ), angle90degrees ); // 
			Matrix4 Tview = Matrix4.CreatePerspectiveFieldOfView( 1.2f, 1.3f, .1f, 1000 );

			// update rotation
			a += 0.001f * frameDuration;
			if( a > 2 * PI ) a -= 2 * PI;

			if( useRenderTarget )
			{
				// enable render target
				target.Bind();


				// render scene to render target

				mesh.Render( shader, scene.Render(mesh)* Tview, gold);
			//	cube.Render(shader, scene.Render(cube) * Tview, purple);
				floor.Render( shader, scene.Render(floor)* Tview, ground );
				table.Render(shader, scene.Render(table) * Tview, wood);
				kop.Render(shader, scene.Render(kop) * Tview, silver);
				kop2.Render(shader, scene.Render(kop2) * Tview, silver);
				//	boom.Render(shader, scene.Render(boom) * Tview, wood);
				//	cloud.Render(shader, scene.Render(cloud) * Tview, wolkig);


				// render quad
				target.Unbind();
				quad.Render( postproc, target.GetTextureID() );
			}
			else
			{
				// render scene directly to the screen

				mesh.Render(shader, scene.Render(mesh)* Tview, wood);
				cube.Render(shader, scene.Render(cube)* Tview, purple);
				floor.Render(shader, scene.Render(floor)* Tview, ground);
				kop.Render(shader, scene.Render(kop) * Tview, wood);

			}
		}

		public void MoveCamera(char direction, bool rotate)
        {
			scene.move(direction, rotate);
        }

		public static bool LightOn()
		{
			var state = OpenTK.Input.Keyboard.GetState();
			if (state[OpenTK.Input.Key.L])
			{
				return true;
			}
			else 
				return false;
		}
	}
}