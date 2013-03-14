using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Ways to Go Green",
                 "Ways to Go Green",
                 "Assets/10.jpg",
                 "How can we live lightly on the Earth and save money at the same time? Staff members at the Worldwatch Institute, a global environmental organization, share ideas on how to GO GREEN and SAVE GREEN at home and at work. To learn more about Worldwatch's efforts to create am environmentally sustainable society that meets human needs");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Save energy to save money",
                 "Save energy to save money",
                 "Assets/11.jpg",
                 "How can we live lightly on the Earth and save money at the same time? Staff members at the Worldwatch Institute, a global environmental organization, share ideas on how to GO GREEN and SAVE GREEN at home and at work. To learn more about Worldwatch's efforts to create am environmentally sustainable society that meets human needs",
                 "Set your thermostat a few degrees lower in the winter and a few degrees higher in the summer to save on heating and cooling costs.\nInstall compact fluorescent light bulbs (CFLs) when your older incandescent bulbs burn out.\nUnplug appliances when you're not using them. Or, use a smart power strip that senses when appliances are off and cuts phantom or vampire energy use.\nWash clothes in cold water whenever possible. As much as 85 percent of the energy used to machine-wash clothes goes to heating the water.\nUse a drying rack or clothesline to save the energy otherwise used during machine drying.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Save water to save money",
                 "Save water to save money",
                 "Assets/12.jpg",
                 "Climate change is in the news. It seems like everyone's going green We're glad you want to take action, too. Luckily, many of the steps we can take to stop climate change can make our lives better. Our grandchildren-and their children-will thank us for living more sustainably. Let's start now.",
                 "Take shorter showers to reduce water use. This will lower your water and heating bills too.\nInstall a low-flow showerhead. They don't cost much, and the water and energy savings can quickly pay back your investment.\nMake sure you have a faucet aerator on each faucet. These inexpensive appliances conserve heat and water, while keeping water pressure high.\nPlant drought-tolerant native plants in your garden. Many plants need minimal watering. Find out which occur naturally in your area.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Less Gas",
                 "Less Gas",
                 "Assets/13.jpg",
                 "How can we live lightly on the Earth and save money at the same time? Staff members at the Worldwatch Institute, a global environmental organization, share ideas on how to GO GREEN and SAVE GREEN at home and at work. To learn more about Worldwatch's efforts to create am environmentally sustainable society that meets human needs",
                 "Walk or bike to work. This saves on gas and parking costs while improving your cardiovascular health and reducing your risk of obesity.\nConsider telecommuting if you live far from your work. Or move closer. Even if this means paying more rent, it could save you money in the long term.\nLobby your local government to increase spending on sidewalks and bike lanes. With little cost, these improvements can pay huge dividends in bettering your health and reducing traffic.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Eat Smart",
                 "Eat Smart.",
                 "Assets/14.jpg",
                 "How can we live lightly on the Earth and save money at the same time? Staff members at the Worldwatch Institute, a global environmental organization, share ideas on how to GO GREEN and SAVE GREEN at home and at work. To learn more about Worldwatch's efforts to create am environmentally sustainable society that meets human needs",
                 "If you eat meat, add one meatless meal a week. Meat costs a lot at the store-and it's even more expensive when you consider the related environmental and health costs.Buy locally raised, humane, and organic meat, eggs, and dairy whenever you can. Purchasing from local farmers keeps money in the local economy.Watch videos about why local food and sustainable seafood are so great.Whatever your diet, eat low on the food chain. This is especially true for seafood.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Skip the bottled water",
                 "Skip the bottled water",
                 "Assets/15.jpg",
                 "Go Green is in the news. It seems like everyone's going green. We're glad you want to take action, too. Luckily, many of the steps we can take to stop climate change can make our lives better. Our grandchildren-and their children-will thank us for living more sustainably. Let's start now.",
                 "Use a water filter to purify tap water instead of buying bottled water. Not only is bottled water expensive, but it generates large amounts of container waste.\nBring a reusable water bottle, preferably aluminum rather than plastic, with you when traveling or at work.\nCheck out this short article for the latest on bottled water trends.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Think before you buy",
                 "Think before you buy",
                 "Assets/16.jpg",
                 "How can we live lightly on the Earth and save money at the same time? Staff members at the Worldwatch Institute, a global environmental organization, share ideas on how to GO GREEN and SAVE GREEN at home and at work. To learn more about Worldwatch's efforts to create am environmentally sustainable society that meets human needs",
                 "Go online to find new or gently used secondhand products. Whether you've just moved or are looking to redecorate, consider a service like craigslist or FreeSharing to track down furniture, appliances, and other items cheaply or for free.\nCheck out garage sales, thrift stores, and consignment shops for clothing and other everyday items.When making purchases, make sure you know what's Good Stuff and what isn't.Watch a video about what happens when you buy things. Your purchases have a real impact, for better or worse.",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Benefits of Going Green",
                 "Benefits of Going Green",
                 "Assets/20.jpg",
                 "Basically, green living refers to a way of life that contributes towards maintaining the natural ecological balance in the environment, and preserving the planet and its natural systems and resources.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Lower costs",
                 "Lower costs",
                 "Assets/21.jpg",
                 "One of the most evident benefits of going green is that it can help us cut cost, whether as an individual, a household, a community or a nation.",
                 "One of the most evident benefits of going green is that it can help us cut cost, whether as an individual, a household, a community or a nation.\n\nWhen you and I conserve energy and resources, by not wasting water and electricity and adopting green traveling tips, we help to reduce the amount of money that needs to be spent on energy and resources.\n\nBy sending our unwanted items for recycling, and supporting the recycling industry by purchasing recycled products, in the long run as a community, we are reducing the cost of production, because it uses more energy (and hence is more costly) to manufacture products using virgin raw materials.\n\nAs we reduce our waste, by reducing consumption or reusing, or diverting waste from the landfills and incinerators through recycling, we would be able to save on waste disposal. Valuable land originally intended for landfills and incinerator plants can now be freed up for other uses.\n\nAnd reducing the pollution that we create is definitely cheaper than trying to clear up the pollution after we have created the mess!\n\nThese are just some of the economic benefits of going green. (Also read about the economic benefits of recycling). You can only stand to benefit by living a green life. So why not start now?",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Healthier lives",
                 "Healthier lives",
                 "Assets/22.jpg",
                 "Are you aware that one of the benefits of going green is a healthier environment for you and me?",
                 "Are you aware that one of the benefits of going green is a healthier environment for you and me?\n\nAs more and more people seek to live a green life by reducing the pollution and carbon footprints they leave behind, we can look forward to better quality air, a cleaner environment and better health.\n\nThe quality of our air definitely has an impact on our health. According to the World Health Organization, air pollution is estimated to cause about 2 million premature deaths worldwide every year. Common air pollutants like lead have been found to be associated with behavioral problems, learning deficits and lowered IQ in young children.In addition, the health of our environment also has an impact on the quality of our food and ultimately our health. It would be hard to imagine how we could remain healthy if we are consuming polluted drinking water and food contaminated with chemicals (eg. fish with heavy metal contamination, vegetables exposed to acid rain, etc) for long periods.\n\nBy keeping our air and environment, cleaner, we are actually building a healthier environment for ourselves, our loved ones and our future generations.\n\nWhen the human race strives towards living a green life on earth, we would have less fear of the impact of extreme temperatures and climate changes brought about by global warming.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "More sustainable world",
                 "More sustainable world",
                 "Assets/23.jpg",
                 "One of the most important benefits of going green is a more sustainable world.",
                 "One of the most important benefits of going green is a more sustainable world.\n\nAt the rate that we are consuming the world’s resources, polluting the earth and fueling global warming, and destroying the earth’s ecosystem, in no time, we would be left with nothing (no clean air, water, land and food) but an un-livable world beyond repair.\n\nOn the day that we reach such a stage, there would be no material comfort or economic growth to talk about at all. This is because our very lives would be threatened – there would be no clean air, no clean drinking water, no food, and probably even no safety from the harsh climate that would have changed beyond recognition.\n\nWhat a horrible situation to be in!\n\nWe need to do something, starting today, if we do not wish to end up in that horrible state.\n\nWhile it is important for each of our nations to seek and maintain growth and development, we need to do so in a sustainable way – via sustainable living. We need to start adopting green practices in our daily lives, as well as encourage others around us to do the same.\n\nIt is important for each and every one of us to do our parts, but an individual’s effort alone is not enough. We need to work together as a human community.\n\nWe must start now!",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Better quality of life",
                 "Better quality of life",
                 "Assets/24.jpg",
                 "Living a green life can actually offer us a better quality of life on earth.To truly experience the benefits of going green, you need to look beyond material luxuries.",
                 "Living a green life can actually offer us a better quality of life on earth.To truly experience the benefits of going green, you need to look beyond material luxuries.\n\nThis is because two of the very key principles of going green are actually conservation and the reduction of consumption and waste – which means to use and consume the earth’s resources with care, and not over-indulge in excessive material luxuries.\n\nConsidering that material luxuries can only bring you momentary happiness, and that at the end of the day, other non-material things (eg. relationships, self-actualisation, etc) actually matter more, looking beyond material luxuries may not be so hard.\n\nNevertheless, that is not to say that going green means to give up the comfortable life you have now.\n\nIt just means having more consideration for the things you use and the environment you live in, bearing in mind the impact of your actions on the earth, and taking a little effort and creativity to reduce the negative impact you leave behind on this earth.\n\nWith a greener planet, you and I can look forward to a cleaner and more beautiful environment, relatively free of pollution. With better quality of air, surroundings and food, we are more likely to be healthier (to be around to fulfill our aspirations and enjoy our relationship with our loved ones).\n\nAt the same time, we will be more in touch with the earth we live on. We can get to experience and better appreciate the wonders of the diversity of animal and plant life on this planet.\n\nAll these benefits can actually help you live a fuller life, more than what a life filled only with materials can do.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Development in new areas",
                 "Development in new areas",
                 "Assets/25.jpg",
                 "As more and more people start living a green life, there will be greater drive for developments in the area of green energies, recycling and other green technologies, as well as a market for eco-friendly products and services.",
                 "As more and more people start living a green life, there will be greater drive for developments in the area of green energies, recycling and other green technologies, as well as a market for eco-friendly products and services.In turn, these developments would make it easier for people to adopt green living practices. A positive reinforcement loop would be created for a green way of life.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Go Green Asset",
                 "Go Green Asset",
                 "Assets/26.jpg",
                 "There are many benefits of going green. But before examining the benefits, it is important for you to understand what going green means.",
                 "There are many benefits of going green. But before examining the benefits, it is important for you to understand what going green means.Basically, green living refers to a way of life that contributes towards maintaining the natural ecological balance in the environment, and preserving the planet and its natural systems and resources.\n\nThere are many things you can do to live a green life, amongst which you can help by:\n\nreducing pollution ,\nconserving natural resources,\nrecycling non-biodegradable products,\ncontributing to conservation of forests and wildlife,\ncultivating more plants and trees in vacant lands, and\nhelping to maintain the ecological balance on the earth, so that all living beings can survive and thrive in their natural habitat.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
