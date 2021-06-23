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
        Vector3 eye = new Vector3(100, 0, 50);
        Vector3 target = new Vector3(0, 0, -10);
        Matrix4 camera, toWorld;
        static public Dictionary<Mesh, Mesh> hierachy = new Dictionary<Mesh, Mesh>();


        public SceneGraph()
        {
            camera = Matrix4.LookAt(eye, target, new Vector3(0, 1, 0));
        }

        public void move(char direction)
        {
            switch(direction)
            {
                case 'w': eye.Y += 1;
                    target.Y += 1;
                    break;

                case 'a': eye.Z += 1;
                    target.Z += 1;
                    break;
                case 's': eye.Y -= 1;
                    target.Y -= 1;
                    break;
                case 'd': eye.Z -= 1;
                    target.Z -= 1;
                    break;
            }
            camera = Matrix4.LookAt(eye, target, new Vector3(0, 1, 0));

        }


        //Hoe gaan we die hierachy implementeren?
        public Matrix4 Render(Mesh child)
        {
            Matrix4 sum;
            if (hierachy.ContainsKey(child))
            {
                Mesh parent = hierachy[child];
                sum = Render(parent) * child.view;
            }

            else
            {
                sum = child.view * camera;
            }


            return sum;
            //Render child
            //Call Render function met child

        }
    }
}
