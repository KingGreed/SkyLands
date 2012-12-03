using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Mogre;

namespace Game.Display
{
    class Materials
    {
        public XmlDocument mDoc;

        public Materials(){
            this.mDoc = new XmlDocument();
            this.mDoc.Load(System.IO.Directory.GetCurrentDirectory() + @"../../../Media/materials/xml/textures.xml");

        }

        public string getMaterial(string id, GraphicBlock.blockFace face){
            
            string XmlFace = this.getFaceName(face);
            XmlNodeList cube = this.mDoc.SelectNodes("cube");
           

            foreach (XmlNode cubes in cube){
                foreach (XmlNode textures in cubes.ChildNodes) {
                    if(id == textures.Attributes["id"].Value){
                        foreach (XmlNode sides in textures.ChildNodes) {
                            if(sides.Name == XmlFace){ return sides.InnerText; }
                        }
                    }
                }
            }

            LogManager.Singleton.DefaultLog.LogMessage("Name Done");

            throw new NotImplementedException("Textures has not been implemented");
        }

        private string getFaceName(GraphicBlock.blockFace face){
            if(face == GraphicBlock.blockFace.underFace){ return "bottom";}
            if(face == GraphicBlock.blockFace.upperFace){ return "top";   }
            else                                        { return "side";}
        }
    }
}
