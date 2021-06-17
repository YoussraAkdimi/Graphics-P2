using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template;

namespace INFOGR2019Tmpl8
{
    class SceneGraph
    {

        static public Dictionary<Mesh, Mesh> hierachy = new Dictionary<Mesh, Mesh>();


        //Hoe gaan we die hierachy implementeren?
        public Matrix4 Render(Mesh child)
        {
            Matrix4 sum;
            if (hierachy.ContainsKey(child))
            {
                Mesh parent = hierachy[child];
                sum = Render(parent) * child.view;
            }
            
            else sum = child.view;
            return sum;
            //Render child
            //Call Render function met child

        }
    }
}
