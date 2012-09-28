using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Krempel.WP7.Core.WebBrowserHelper;
using System.Windows.Navigation;

namespace Krempel.WP7.TestApp.Examples
{
    public partial class HtmlTextBlockExample : PhoneApplicationPage
    {
        public HtmlTextBlockExample()
        {
            InitializeComponent();

            this.DataContext = @"<p><img src='http://ecn.channel9.msdn.com/o9/content/images/emoticons/emotion-1.gif?v=c9' alt='Smiley' />Today's Mobile Monday (note: in the future, there might also be Metro Monday projects too... but that's for the future...) is a simple, straight forward and very educational series for creating a XNA based game for Windows Phone 7</p><h2><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/xna-for-windows-phone-walkthrough-creating-the-bizzy-bees-game.aspx"" target=""_blank"">XNA for Windows Phone Walkthrough–Creating the Bizzy Bees game</a></h2><blockquote><p>The game is called Bizzy Bees and you can download and play it for free from the <a href=""http://www.windowsphone.com/en-us/apps/403e83f4-9371-e011-81d2-78e7d1fa76f8"">Marketplace</a>.</p><p><a href=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/image%5B2%5D-39.png"" target=""_blank""><img title=""image"" src=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/image_thumb-38.png"" alt=""image"" width=""244"" height=""407"" border=""0""></a></p><p>The idea of the app is quite simple… the goal is to collect as many rainbow flowers as you can before all the flowers hit the bottom. You collect flowers by matching flowers and bees, so a yellow flower matches with a yellow be, a pink flower with a pink bee etc. and all bees match up with rainbow flowers</p><p>In this series we’ll walk through creating a subset of that game from start to finish.</p><p><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-1-setting-the-stage-xna-walkthrough.aspx"">Step 1: Setting the stage (projects and assets)</a><br><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-2-drawing-the-scene-xna-walkthrough.aspx"">Step 2: Drawing the scene</a><br><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-3-adding-flowers-xna-walkthrough.aspx"">Step 3: Adding flowers</a><br><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-4-making-things-move-xna-walkthrough.aspx"">Step 4: Making things move</a><br><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-5-adding-some-bees-to-the-mix-xna-walkthrough.aspx"">Step 5: Adding some bees to the mix</a><br><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-6-user-interaction-xna-walkthrough.aspx"">Step 6: User interaction</a><br><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-7-rounding-it-up-xna-walkthrough.aspx"">Step 7: Rounding it up</a></p><p>...</p></blockquote><p>What's coolest about this, besides the source we get to play with (see Solution below) is that <a href=""http://blogs.msdn.com/b/tess/"" target=""_blank"">Tess</a> walks us through all the steps in creating the game...</p><p><a href=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/image%5B6%5D-30.png"" target=""_blank""><img title=""image"" src=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/image_thumb%5B2%5D-38.png"" alt=""image"" width=""244"" height=""427"" border=""0""></a></p><p>From a short introduction to writing games in XNA (which is very different than Silverlight) and getting the initial assets (i.e. bitmaps, etc.) setup in, <a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-1-setting-the-stage-xna-walkthrough.aspx"" target=""_blank"">Bizzy Bees Step 1: Setting the stage (XNA walkthrough)</a>;</p><p><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-1-setting-the-stage-xna-walkthrough.aspx"" target=""_blank""><img title=""SNAGHTML991fcd7"" src=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/SNAGHTML991fcd7%5B4%5D.png"" alt=""SNAGHTML991fcd7"" width=""461"" height=""407"" border=""0""></a>&nbsp;</p><p>To making stuff actually move in <a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-4-making-things-move-xna-walkthrough.aspx"" target=""_blank"">Bizzy Bees Step 4: Making things move (XNA Walkthrough)</a>;</p><p><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-4-making-things-move-xna-walkthrough.aspx"" target=""_blank""><img title=""SNAGHTML9926662"" src=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/SNAGHTML9926662%5B4%5D.png"" alt=""SNAGHTML9926662"" width=""468"" height=""407"" border=""0""></a>&nbsp;</p><p>And onto the final touches, homework for the reader in <a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-7-rounding-it-up-xna-walkthrough.aspx"">Bizzy Bees Step 7: Rounding it up (XNA Walkthrough)</a>;</p><p><a href=""http://blogs.msdn.com/b/tess/archive/2012/03/02/bizzy-bees-step-7-rounding-it-up-xna-walkthrough.aspx"" target=""_blank""><img title=""SNAGHTML992da59"" src=""http://files.channel9.msdn.com/wlwimages/ae054c0b4d7b402ab1239e6800c0220f/SNAGHTML992da59%5B4%5D.png"" alt=""SNAGHTML992da59"" width=""503"" height=""407"" border=""0""></a></p><p>In just about every post Tess provides code snips, graphics, details, how to avoid common problems/issues and commentary related to the task at hand.</p><p>If you're looking for a friendly and easy to follow tutorial series for creating XNA based games for Windows Phone 7, this is a series you're going to want to read.</p> <img src=""http://m.webtrends.com/dcs1wotjh10000w0irc493s0e_6x1g/njs.gif?dcssip=channel9.msdn.com&dcsuri=http://channel9.msdn.com/coding4fun/blog/feed&WT.dl=0&WT.entryid=Entry:RSSView:0889e420df3042c0bb2ea00f0124293f"">";
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            nonFormattedBrowser.NavigateToString(DataContext.ToString());

            formattedBrowser.NavigateToString(WebBrowserHelper.WrapHtml(DataContext.ToString(), 480));
        }

        private void htmlTextBlock_NavigationRequested(object sender, NavigationEventArgs e)
        {
            // due to the internal workings of the hyperlink, command will execute first (if attached) and event will be ignored.
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            this.TestHTMLTextBox.Html= "<h1>Header</h1> just text <h2>Header2</h2> <a href='http://www.google.com'>Hyperlink</a>";
        }
    }
}