using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template;

//https://www.scratchapixel.com/lessons/mathematics-physics-for-computer-graphics/lookat-function

namespace INFOGR2019Tmpl8
{
    class SceneGraph
    {
        Vector3 eye = new Vector3(100, 50, 150);
        Vector3 target = new Vector3(0, 0, -10);
        Matrix4 camera, toWorld;
        static public Dictionary<Mesh, Mesh> hierachy = new Dictionary<Mesh, Mesh>();
        public int width, height;


        public SceneGraph(Surface screen)
        {
            width = screen.width;
            height = screen.height;
            camera = Matrix4.LookAt(eye, target, new Vector3(0, 1, 0));
        }

        public void move(char direction, bool rotate)
        {
            if (rotate)
            {
                switch (direction)
                {
                    case 'w':
                       
                        target.Y += 10;
                        break;

                    case 'a':
                        target.X -= 10;
                        break;

                    case 's':
                        target.Y -= 10;
                        break;

                    case 'd':
                        target.X += 10;// + (float)Math.Sin(0.001f);
                        break;
                }
            }
            else
            {


                switch (direction)
                {
                    case 'w':
                        eye.Y += 1;
                        target.Y += 1;
                        break;

                    case 'a':
                        eye.Z += 1;
                        target.Z += 1;
                        break;
                    case 's':
                        eye.Y -= 1;
                        target.Y -= 1;
                        break;
                    case 'd':
                        eye.Z -= 1;
                        target.Z -= 1;
                        break;
                }
            }
            camera = Matrix4.LookAt(eye, target, new Vector3(0, 1, 0));

        }


        /// <summary>
        /// Should return true if the mesh is outside the screen
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="target"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool Culling(Mesh child)
        {
            for (int j = 0; j < 4; j++)
            {
                Plane plane = Frustum(j);
                //Console.WriteLine(vertices.Length);
                for (int i = 0; i < child.vertices.Length; i += 1)
                {

                    Vector3 punt = (child.vertices[i].Vertex - eye);
                    Matrix3 matrix3 = new Matrix3(plane.vector.X, plane.vector2.X, punt.X, plane.vector.Y, plane.vector2.Y, punt.Y, plane.vector.Z, plane.vector2.Z, punt.Z);
                    //	Console.WriteLine(matrix3.Determinant + " Nummer "+ j);
                    if (matrix3.Determinant > 0) return false;


                }
            }

            return true;
        }

        Plane Frustum(int nummer)
        {
            Plane zijde = new Plane();
            switch (nummer)
            {
                case 0: // links is buiten beeld
                    zijde.point = eye;
                    zijde.vector = new Vector3(target.X - width / 2, target.Y - height / 2, target.Z) - eye;
                    zijde.vector2 = new Vector3(target.X - width / 2, target.Y + height / 2, target.Z) - eye;
                    break;
                case 1: // links is buiten beeld
                    zijde.point = eye;
                    zijde.vector = new Vector3(target.X + width / 2, target.Y + height / 2, target.Z) - eye;
                    zijde.vector2 = new Vector3(target.X + width / 2, target.Y - height / 2, target.Z) - eye;
                    break;
                case 2:
                    zijde.point = eye;
                    zijde.vector = new Vector3(target.X + width / 2, target.Y - height / 2, target.Z) - eye;
                    zijde.vector2 = new Vector3(target.X - width / 2, target.Y - height / 2, target.Z) - eye;
                    break;
                case 3:
                    zijde.point = eye;
                    zijde.vector = new Vector3(target.X - width / 2, target.Y + height / 2, target.Z) - eye;
                    zijde.vector2 = new Vector3(target.X + width / 2, target.Y + height / 2, target.Z) - eye;
                    break;
                case 4:
                    zijde.point = eye;
                    zijde.vector = new Vector3(0, 0, 10) - eye;
                    zijde.vector2 = new Vector3(0, -height, 10) - eye;
                    break;
                case 5:
                    zijde.point = eye;
                    zijde.vector = new Vector3(0, 0, 10) - eye;
                    zijde.vector2 = new Vector3(0, -height, 10) - eye;
                    break;

            }
            return zijde;
        }
        //Hoe gaan we die hierachy implementeren?
        public Matrix4 Render(Mesh child)
        {
            Matrix4 sum;
            if (hierachy.ContainsKey(child))
            {
                Mesh parent = hierachy[child];
                sum = child.view * Render(parent);
            }

            else
            {
                sum = child.view * camera;
            }

            if (Culling(child)) //Object valt buiten beeld.
            {
                Console.WriteLine("Offscreen" + child.vertices.Length);
                sum = Matrix4.Zero;
            }
            return sum;
            //Render child
            //Call Render function met child

        }
        public struct Plane
        {
            public Vector3 point;
            public Vector3 vector;
            public Vector3 vector2;
        }
    }
}
