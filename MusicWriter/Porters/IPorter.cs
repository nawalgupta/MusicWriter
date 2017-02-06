using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
	public interface IPorter
	{
		string Name { get; }

		string FileExtension { get; }

		void Import(
                EditorFile file,
                string filename,
                PorterOptions options
            );

		void Export(
                EditorFile file,
                string filename,
                PorterOptions options
            );
	}
}
