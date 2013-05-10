using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Televic.Controls
{
	public delegate void MapItemChangedHandler(object sender, Point newLocation);

	public delegate void MapItemPopedUpHandler(object sender);

	public delegate void MapItemPopedDownHandler(object sender);
}
