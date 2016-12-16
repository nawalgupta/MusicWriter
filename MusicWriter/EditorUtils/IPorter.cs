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

		void Import<View>(
                EditorFile<View> editor,
                string filename,
                PorterOptions options
            );

		void Export<View>(
                EditorFile<View> editor,
                string filename,
                PorterOptions options
            );
	}
}
