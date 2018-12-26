using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class SymSet {
        HashSet<Type> types;

        public SymSet() {
            this.types = new HashSet<Type>();
        }

        public void UnionWith(SymSet s) {
            this.types.UnionWith(s.types);
        }

        public bool Contains(Type t) {
            return this.types.Contains(t);
        }

        public void Add(params Type[] types) {
            foreach (Type t in types)
                if (!this.types.Contains(t))
                    this.types.Add(t);
        }
    }
}
