using System.Diagnostics;
using INFOGR2019Tmpl8;
using OpenTK;
using System;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
		static public Mesh mesh, floor, cube, cloud, boom;            // a mesh to draw using OpenGL
		const float PI = 3.1415926535f;         // PI
		float a = 0;                            // teapot rotation angle
		Stopwatch timer;                        // timer for measuring frame duration
		Shader shader;                          // shader to use for rendering
		Shader postproc;                        // shader to use for post processing
		Texture wood, ground, purple, wolkig;                           // texture to use for rendering
		RenderTarget target;                    // intermediate render target
		ScreenQuad quad;                        // screen filling quad for post processing
		bool useRenderTarget = true;
		SceneGraph scene = new SceneGraph();
		// initialize
		public void Init()
		{
			// load teapot
			mesh = new Mesh( "../../assets/teapot.obj" );
			floor = new Mesh( "../../assets/floor.obj" );
			cube = new Mesh("../../assets/cube.obj");
			cloud = new Mesh("../../assets/cumulus00.obj");
			boom = new Mesh("../../assets/AL05m");
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
			// create the render target
			target = new RenderTarget( screen.width, screen.height );
			quad = new ScreenQuad();
			// create the hierachy
			SceneGraph.hierachy.Add(mesh, floor);
			SceneGraph.hierachy.Add(cube, floor);

		}

		// tick for background surface
		public void Tick()
		{
			screen.Clear( 0 );
			screen.Print( "hello world", 2, 2, 0xffff00 );
		}

		// tick for OpenGL rendering code
		public void RenderGL()
		{
			// measure frame duration
			float frameDuration = timer.ElapsedMilliseconds;
			timer.Reset();
			timer.Start();

			// prepare matrix for vertex shader
			float angle90degrees = PI / 2;
			mesh.view = Matrix4.CreateScale(0.5f / 6) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0) * Matrix4.CreateTranslation(new Vector3((float)(5 * Math.Cos(a)), -2.0f, -(float)(5 * Math.Sin(a)))); ;	//Camera transformation.
			floor.view = Matrix4.CreateScale( 8.0f ) * Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );
			cube.view = Matrix4.CreateScale(2/4f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
			cloud.view = Matrix4.CreateScale(1.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
			boom.view = Matrix4.CreateScale(1.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
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

				mesh.Render( shader, scene.Render(mesh)* Tview, wood );
				cube.Render(shader, scene.Render(cube) * Tview, purple);
				floor.Render( shader, scene.Render(floor)* Tview, ground );
				//boom.Render(shader, scene.Render(boom) * Tview)
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

			}
		}

		public void MoveCamera(char direction)
        {
			scene.move(direction);
        }
	}
}