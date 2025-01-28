using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.Helper;

public static class UIHelper
{
    static UIHelper()
    {
    }

    public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject start) where T : DependencyObject
    {
        return start.GetDescendants().OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
    {
        var queue = new Queue<DependencyObject>();
        var count1 = VisualTreeHelper.GetChildrenCount(start);

        for (int i = 0; i < count1; i++)
        {
            var child = VisualTreeHelper.GetChild(start, i);
            yield return child;
            queue.Enqueue(child);
        }

        while (queue.Count > 0)
        {
            var parent = queue.Dequeue();
            var count2 = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count2; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                yield return child;
                queue.Enqueue(child);
            }
        }
    }

    static public UIElement FindElementByName(UIElement element, string name)
    {
        if (element.XamlRoot != null && element.XamlRoot.Content != null)
        {
            var ele = (element.XamlRoot.Content as FrameworkElement).FindName(name);
            if (ele != null)
            {
                return ele as UIElement;
            }
        }
        return null;
    }

    // Confirmation of Action
    static public void AnnounceActionForAccessibility(UIElement ue, string annoucement, string activityID)
    {
        var peer = FrameworkElementAutomationPeer.FromElement(ue);
        peer.RaiseNotificationEvent(AutomationNotificationKind.ActionCompleted,
                                    AutomationNotificationProcessing.ImportantMostRecent, annoucement, activityID);
    }

    public static T GetParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject current = child;

        while (current != null)
        {
            if (current is T parent)
            {
                return parent;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
