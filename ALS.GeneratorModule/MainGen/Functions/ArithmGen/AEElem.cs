using System.Text;

namespace Generator
{
    public class AEElem
    {
        public string Name { get; set; }
        private string _model{ get; set; }
        private string _modelMD { get; set; }
        private string _constraints { get; set; }

        public AEElem(string name, string model, string modelMD, string constraints)
        {
            Name = name;
            _model = model;
            _modelMD = modelMD;
            _constraints = constraints;
        }

        private string Make(Tree tree, string model)
        {
            if (model == default) return model;

            StringBuilder res = new StringBuilder(model);
            res = res.Replace("?L", tree.Left.Value);
            if (tree.Right != null) res = res.Replace("?R", tree.Right.Value);

            return res.ToString();
        }

        public string MakeMD(Tree tree) 
        {
            StringBuilder res = new StringBuilder(_modelMD);
            res = res.Replace("?L", tree.Left.ValueMD);
            if (tree.Right != null) res = res.Replace("?R", tree.Right.ValueMD);

            return res.ToString();
        }
        public string MakeOrig(Tree tree) => Make(tree, _model);
        public string MakeConstr(Tree tree) => Make(tree, _constraints);

    }
}
