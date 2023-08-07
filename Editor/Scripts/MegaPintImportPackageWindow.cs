using System.Collections.Generic;
using Editor.Scripts.Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public class MegaPintImportPackageWindow : MegaPintEditorWindowBase
    {
        private const string ListItemTemplate = "User Interface/Import/MegaPintPackageItem";
        
        /// <summary> Loaded reference of the uxml </summary>
        private VisualTreeAsset _baseWindow;

        private VisualTreeAsset _listItem;

        private GroupBox _rightPane;
        private ListView _list;
        private Label _packageName;
        private Label _lastUpdate;
        private Label _unityVersion;
        private Label _megaPintVersion;

        private List<MegaPintPackagesData.MegaPintPackageData> _displayedPackages;
        private MegaPintPackagesData.MegaPintPackageData _selectedPackage;

        #region Overrides

        protected override string BasePath() => "User Interface/Import/MegaPintImportPackageWindow";

        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "Import Package";
            return this;
        }
        
        protected override void CreateGUI()
        {
            base.CreateGUI();

            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            _list = content.Q<ListView>("MainList");

            _list.makeItem = () => _listItem.Instantiate();

            _list.bindItem = UpdateItem;

            _list.unbindItem = (element, _) => element.Clear();

            _list.onSelectedIndicesChange += _ => UpdateRightPane();

            _displayedPackages = MegaPintPackagesData.Packages;
            _list.itemsSource = _displayedPackages;

            _rightPane = content.Q<GroupBox>("RightPane");
            _packageName = _rightPane.Q<Label>("PackageName");
            _lastUpdate = _rightPane.Q<Label>("LastUpdate");
            _unityVersion = _rightPane.Q<Label>("UnityVersion");
            _megaPintVersion = _rightPane.Q<Label>("MegaPintVersion");

            UpdateRightPane();

            root.Add(content);
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            _listItem = Resources.Load<VisualTreeAsset>(ListItemTemplate);
            return _baseWindow != null && _listItem != null;
        }

        protected override void LoadSettings() { }

        #endregion

        private void UpdateItem(VisualElement element, int index)
        {
            element.Q<Label>("PackageName").text = _displayedPackages[index].PackageNiceName;
            element.Q<Label>("Version").text = _displayedPackages[index].Version;
        }

        private void UpdateRightPane()
        {
            var content = _rightPane.Q<GroupBox>("Content");
            var index = _list.selectedIndex;

            if (index < 0)
            {
                content.style.display = DisplayStyle.None;
                return;
            }
            
            content.style.display = DisplayStyle.Flex;
            
            var package = _displayedPackages[index];
            _packageName.text = package.PackageNiceName;
            _lastUpdate.text = package.LastUpdate;
            _unityVersion.text = package.UnityVersion;
            _megaPintVersion.text = package.MegaPintVersion;
        }
    }
}
