using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    class NotificationLink
    {
        INotifyPropertyChanged Source;
        string SourceProperty;
        IPushNotification Target;
        string TargetProperty;

        static List<NotificationLink> Registry = new List<NotificationLink>();

        public NotificationLink(INotifyPropertyChanged source, string sourceProperty, IPushNotification target, string targetProperty)
        {
            Source = source;
            SourceProperty = sourceProperty;
            Target = target;
            TargetProperty = targetProperty;

            foreach (NotificationLink other in Registry)
            {
                if (other.Source == Source && other.SourceProperty == SourceProperty
                    && other.Target == Target && other.TargetProperty == TargetProperty)
                {
                    return;
                }
            }

            Registry.Add(this);

            source.PropertyChanged += Notify;
        }

        public void Notify(object source, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == SourceProperty)
            {
                Source.PropertyChanged -= Notify;

                Registry.Remove(this);

                Target.PushNotification(TargetProperty);
            }
        }
    }
}
