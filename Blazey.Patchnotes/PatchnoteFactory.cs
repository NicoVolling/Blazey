using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazey.Patchnotes;
public abstract class PatchnoteFactory
{
    public PatchnoteFactory()
    {
        Patchnote = Construct();
    }

    public Patchnote Patchnote { get; init; }

    public abstract Patchnote Construct();
}
