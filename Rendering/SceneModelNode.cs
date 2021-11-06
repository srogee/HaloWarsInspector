using OpenTK.Mathematics;

namespace HaloWarsInspector.Rendering
{
    public class SceneModelNode : SceneNode
    {
        public Model Model;

        public SceneModelNode(Model model) : base() {
            Model = model;
        }

        protected override void Draw(Matrix4 model, Matrix4 view, Matrix4 projection) {
            base.Draw(model, view, projection);
            Model.Draw(model, view, projection);
        }
    }
}
