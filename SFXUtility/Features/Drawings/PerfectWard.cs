﻿#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 PerfectWard.cs is part of SFXUtility.

 SFXUtility is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 SFXUtility is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with SFXUtility. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion License

namespace SFXUtility.Features.Drawings
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Classes;
    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Common.Data;
    using SFXLibrary;
    using SFXLibrary.Logger;
    using SharpDX;
    using Color = System.Drawing.Color;

    #endregion

    internal class PerfectWard : Base
    {
        private const float CheckInterval = 300f;

        private readonly List<int> _greenWards = new List<int>
        {
            ItemData.Warding_Totem_Trinket.Id,
            ItemData.Wriggles_Lantern.Id,
            ItemData.Sightstone.Id,
            ItemData.Ruby_Sightstone.Id,
            ItemData.Explorers_Ward.Id
        };

        private readonly List<int> _pinkWards = new List<int> {ItemData.Stealth_Ward.Id, ItemData.Greater_Stealth_Totem_Trinket.Id};
        // Credits: DrunkenNinja

        #region Ward Spots

        private readonly List<WardSpot> _wardSpots = new List<WardSpot>
        {
            new WardSpot(new Vector3(9551.15f, 137.19f, 60.83f)),
            // Top Blue Inhibitor Turret
            new WardSpot(new Vector3(11398.03f, 1440.97f, 50.64f)), // Blue Bot Outer Turret
            new WardSpot(new Vector3(4281.39f, 4467.51f, 71.63f)), // Blue Midlane Inhibitor Turret
            new WardSpot(new Vector3(2757.31f, 5194.54f, 52.94f)), // Blue North Gate
            new WardSpot(new Vector3(5172.51f, 3116.71f, 51.05f)), // Blue South Gate
            new WardSpot(new Vector3(5488.53f, 5657.80f, 51.03f)), // Blue Midlane inner Turret 
            new WardSpot(new Vector3(9176.99f, 9367.65f, 52.51f)), // Purple Midlane inner Turret
            new WardSpot(new Vector3(12017.83f, 9787.35f, 52.30f)), // Purple South Gate
            new WardSpot(new Vector3(9704.83f, 11848.35f, 52.30f)), // Purple North Gate
            new WardSpot(new Vector3(3261.93f, 7773.65f, 60.0f)), // Blue Golem
            new WardSpot(new Vector3(7831.46f, 3501.13f, 60.0f)), // Blue Lizard
            new WardSpot(new Vector3(10586.62f, 3067.93f, 60.0f)), // Blue Tri Bush
            new WardSpot(new Vector3(6483.73f, 4606.57f, 60.0f)), // Blue Pass Bush
            new WardSpot(new Vector3(7610.46f, 5000.0f, 60.0f)), // Blue River Entrance
            new WardSpot(new Vector3(4717.09f, 7142.35f, 50.83f)), // Blue Round Bush
            new WardSpot(new Vector3(4882.86f, 8393.77f, 27.83f)), // Blue River Round Bush
            new WardSpot(new Vector3(6951.01f, 3040.55f, 52.26f)), // Blue Split Push Bush
            new WardSpot(new Vector3(5583.74f, 3573.83f, 51.43f)), // Blue Riveer Center Close
            new WardSpot(new Vector3(11600.35f, 7090.37f, 51.73f)), // Purple Golem
            new WardSpot(new Vector3(11573.9f, 6457.76f, 51.71f)), // Purple Golem2
            new WardSpot(new Vector3(12629.72f, 4908.16f, 48.62f)), // Purple Tri Bush2
            new WardSpot(new Vector3(7018.75f, 11362.12f, 54.76f)), // Purple Lizard
            new WardSpot(new Vector3(4232.69f, 11869.25f, 47.56f)), // Purple Tri Bush
            new WardSpot(new Vector3(8198.22f, 10267.89f, 49.38f)), // Purple Pass Bush
            new WardSpot(new Vector3(7202.43f, 9881.83f, 53.18f)), // Purple River Entrance
            new WardSpot(new Vector3(10074.63f, 7761.62f, 51.74f)), // Purple Round Bush
            new WardSpot(new Vector3(9795.85f, 6355.15f, -12.21f)), // Purple River Round Bush
            new WardSpot(new Vector3(7836.85f, 11906.34f, 56.48f)), // Purple Split Push Bush
            new WardSpot(new Vector3(10546.35f, 5019.06f, -60.0f)), // Dragon
            new WardSpot(new Vector3(9344.95f, 5703.43f, -64.07f)), // Dragon Bush
            new WardSpot(new Vector3(4334.98f, 9714.54f, -60.42f)), // Baron
            new WardSpot(new Vector3(5363.31f, 9157.05f, -62.70f)), // Baron Bush
            new WardSpot(new Vector3(12731.25f, 9132.66f, 50.32f)), // Purple Bot T2
            new WardSpot(new Vector3(8036.52f, 12882.94f, 45.19f)), // Purple Bot T2
            new WardSpot(new Vector3(9757.9f, 8768.25f, 50.73f)), // Purple Mid T1
            new WardSpot(new Vector3(4749.79f, 5890.76f, 53.59f)), // Blue Mid T1
            new WardSpot(new Vector3(5217.58f, 1288.98f, 77.63f)), // Blue Botlane Inhibitor Turret 
            new WardSpot(new Vector3(1213.70f, 5324.73f, 58.77f)), // Blue Top T2
            new WardSpot(new Vector3(6623.58f, 6973.31f, 51.85f)), // Blue MidLane/outer Turret
            new WardSpot(new Vector3(8120.67f, 8110.15f, 60.0f)), // Purple MidLane/outer Turret
            new WardSpot(new Vector3(9736.8f, 6916.26f, 51.98f)), // Purple Mid Path
            new WardSpot(new Vector3(2222.31f, 9964.1f, 53.2f)), // Blue Tri Top
            new WardSpot(new Vector3(9255.25f, 2183.65f, 57.5f)), // Blue Golem Bush
            new WardSpot(new Vector3(12523.26f, 1635.91f, 57.0f)), // Bot Side Bush1
            new WardSpot(new Vector3(13258.06f, 2463.80f, 51.36f)), // Bot Side Bush2
            new WardSpot(new Vector3(14052.25f, 6988.55f, 52.30f)), // Bot Red Side Bush
            new WardSpot(new Vector3(11786.12f, 4106.47f, -70.71f)), // Bot River Bush
            new WardSpot(new Vector3(3063.29f, 10905.67f, -67.48f)), // Top River Bush
            new WardSpot(new Vector3(2398.38f, 13521.84f, 52.83f)), // Top Side Bush1
            new WardSpot(new Vector3(1711.55f, 12998.53f, 52.49f)), // Top Side Bush2
            new WardSpot(new Vector3(1262.71f, 12310.28f, 52.83f)), // Top Side Bush3
            new WardSpot(new Vector3(805.90f, 8138.49f, 52.83f)), // Top Blue Side Bush
            new WardSpot(new Vector3(7216.65f, 14115.32f, 52.83f)), // Top Red Side Bush
            new WardSpot(new Vector3(5704.763f, 12762.87f, 52.83f)), // Blue Golem Bush
            new WardSpot(new Vector3(9211.42f, 11480.54f, 53.16f)), // Red Base Bush
            new WardSpot(new Vector3(7761.92f, 842.31f, 49.44f)), // Bot Blue Side Bush
            new WardSpot(new Vector3(6526.12f, 8367.01f, -71.21f)), // Mid/Top River Bush
            new WardSpot(new Vector3(8598.335f, 4729.91f, 51.93f)), // Red Buff/River Bush
            new WardSpot(new Vector3(8417.12f, 6521.46f, -71.24f)), // Mid/Top River Bush
            // Dragon -> Tri Bush
            new WardSpot(new Vector3(10072.0f, 3908.0f, -71.24f), new Vector3(10297.93f, 3358.59f, 49.03f), new Vector3(10273.9f, 3257.76f, 49.03f),
                new Vector3(10072.0f, 3908.0f, -71.24f), true),
            // Nashor -> Tri Bush
            new WardSpot(new Vector3(4724.0f, 10856.0f, -71.24f), new Vector3(4627.26f, 11311.69f, -71.24f), new Vector3(4473.9f, 11457.76f, 51.4f),
                new Vector3(4724.0f, 10856.0f, -71.24f), true),

            // Blue Top -> Solo Bush
            new WardSpot(new Vector3(2824.0f, 10356.0f, 54.33f), new Vector3(3078.62f, 10868.39f, 54.33f), new Vector3(3078.62f, 10868.39f, -67.95f),
                new Vector3(2824.0f, 10356.0f, 54.33f), true),

            // Blue Mid -> round Bush // Inconsistent Placement
            new WardSpot(new Vector3(5474.0f, 7906.0f, 51.67f), new Vector3(5132.65f, 8373.2f, 51.67f), new Vector3(5123.9f, 8457.76f, -21.23f),
                new Vector3(5474.0f, 7906.0f, 51.67f), true),

            // Blue Mid -> River Lane Bush
            new WardSpot(new Vector3(5874.0f, 7656.0f, 51.65f), new Vector3(6202.24f, 8132.12f, 51.65f), new Vector3(6202.24f, 8132.12f, -67.39f),
                new Vector3(5874.0f, 7656.0f, 51.65f), true),

            // Blue Lizard -> Dragon Pass Bush
            new WardSpot(new Vector3(8022.0f, 4258.0f, 53.72f), new Vector3(8400.68f, 4657.41f, 53.72f), new Vector3(8523.9f, 4707.76f, 51.24f),
                new Vector3(8022.0f, 4258.0f, 53.72f), true),

            // Purple Mid -> Round Bush // Inconsistent Placement
            new WardSpot(new Vector3(9372.0f, 7008.0f, 52.63f), new Vector3(9703.5f, 6589.9f, 52.63f), new Vector3(9823.9f, 6507.76f, 23.47f),
                new Vector3(9372.0f, 7008.0f, 52.63f), true),

            // Purple Mid -> River Round Bush
            new WardSpot(new Vector3(9072.0f, 7158.0f, 53.04f), new Vector3(8705.95f, 6819.1f, 53.04f), new Vector3(8718.88f, 6764.86f, 95.75f),
                new Vector3(9072.0f, 7158.0f, 53.04f), true),

            // Purple Bottom -> Solo Bush
            new WardSpot(new Vector3(12422.0f, 4508.0f, 51.73f), new Vector3(12353.94f, 4031.58f, 51.73f), new Vector3(12023.9f, 3757.76f, -66.25f),
                new Vector3(12422.0f, 4508.0f, 51.73f), true),

            // Purple Lizard -> Nashor Pass Bush
            new WardSpot(new Vector3(6824.0f, 10656.0f, 56.0f), new Vector3(6370.69f, 10359.92f, 56.0f), new Vector3(6273.9f, 10307.76f, 53.67f),
                new Vector3(6824.0f, 10656.0f, 56.0f), true),

            // Blue Golem -> Blue Lizard
            new WardSpot(new Vector3(8272.0f, 2908.0f, 51.13f), new Vector3(8163.7056f, 3436.0476f, 51.13f), new Vector3(8163.71f, 3436.05f, 51.6628f),
                new Vector3(8272.0f, 2908.0f, 51.13f), true),

            // Red Golem -> Red Lizard
            new WardSpot(new Vector3(6574.0f, 12006.0f, 56.48f), new Vector3(6678.08f, 11477.83f, 56.48f), new Vector3(6678.08f, 11477.83f, 53.85f),
                new Vector3(6574.0f, 12006.0f, 56.48f), true),

            // Blue Top Side Brush
            new WardSpot(new Vector3(1774.0f, 10756.0f, 52.84f), new Vector3(2302.36f, 10874.22f, 52.84f), new Vector3(2773.9f, 11307.76f, -71.24f),
                new Vector3(1774.0f, 10756.0f, 52.84f), true),

            // Mid Lane Death Brush
            new WardSpot(new Vector3(5874.0f, 8306.0f, -70.12f), new Vector3(5332.9f, 8275.21f, -70.12f), new Vector3(5123.9f, 8457.76f, -21.23f),
                new Vector3(5874.0f, 8306.0f, -70.12f), true),

            // Mid Lane Death Brush Right Side
            new WardSpot(new Vector3(9022.0f, 6558.0f, 71.24f), new Vector3(9540.43f, 6657.68f, 71.24f), new Vector3(9773.9f, 6457.76f, 9.56f),
                new Vector3(9022.0f, 6558.0f, 71.24f), true),

            // Blue Inner Turret Jungle
            new WardSpot(new Vector3(6874.0f, 1708.0f, 50.52f), new Vector3(6849.11f, 2252.01f, 50.52f), new Vector3(6723.9f, 2507.76f, 52.17f),
                new Vector3(6874.0f, 1708.0f, 50.52f), true),

            // Purple Inner Turret Jungle
            new WardSpot(new Vector3(8122.0f, 13206.0f, 52.84f), new Vector3(8128.53f, 12658.41f, 52.84f), new Vector3(8323.9f, 12457.76f, 56.48f),
                new Vector3(8122.0f, 13206.0f, 52.84f), true)
        };

        #endregion Ward Spots

        private float _lastCheck = Environment.TickCount;
        private SpellSlot _lastWardSlot = default(SpellSlot);
        private WardSpot _lastWardSpot = default(WardSpot);
        private Drawings _parent;

        public override bool Enabled
        {
            get { return _parent != null && _parent.Enabled && Menu != null && Menu.Item(Name + "Enabled").GetValue<bool>(); }
        }

        public override string Name
        {
            get { return Language.Get("F_PerfectWard"); }
        }

        protected override void OnEnable()
        {
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDrawingDraw;
            Spellbook.OnCastSpell += OnSpellbookCastSpell;
            Obj_AI_Base.OnNewPath += OnObjAiBaseNewPath;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Game.OnUpdate -= OnGameUpdate;
            Drawing.OnDraw -= OnDrawingDraw;
            Spellbook.OnCastSpell -= OnSpellbookCastSpell;
            Obj_AI_Base.OnNewPath -= OnObjAiBaseNewPath;
            base.OnDisable();
        }

        protected override void OnGameLoad(EventArgs args)
        {
            try
            {
                if (Global.IoC.IsRegistered<Drawings>())
                {
                    _parent = Global.IoC.Resolve<Drawings>();
                    if (_parent.Initialized)
                        OnParentInitialized(null, null);
                    else
                        _parent.OnInitialized += OnParentInitialized;
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private void OnParentInitialized(object sender, EventArgs eventArgs)
        {
            try
            {
                if (_parent.Menu == null)
                    return;

                Menu = new Menu(Name, Name);

                var drawingMenu = new Menu(Language.Get("G_Drawing"), Name + "Drawing");
                drawingMenu.AddItem(new MenuItem(drawingMenu.Name + "Color", Language.Get("G_Color")).SetValue(Color.GreenYellow));
                drawingMenu.AddItem(new MenuItem(drawingMenu.Name + "Radius", Language.Get("G_Radius")).SetValue(new Slider(60, 10, 200)));
                drawingMenu.AddItem(
                    new MenuItem(drawingMenu.Name + "CircleThickness", Language.Get("G_CircleThickness")).SetValue(new Slider(2, 1, 10)));


                Menu.AddItem(new MenuItem(Name + "Hotkey", Language.Get("G_Hotkey")).SetValue(new KeyBind('Z', KeyBindType.Press)));

                Menu.AddSubMenu(drawingMenu);

                Menu.AddItem(new MenuItem(Name + "Enabled", Language.Get("G_Enabled")).SetValue(false));

                _parent.Menu.AddSubMenu(Menu);

                HandleEvents(_parent);
                RaiseOnInitialized();
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private void OnDrawingDraw(EventArgs args)
        {
            try
            {
                if (!Menu.Item(Name + "Hotkey").IsActive())
                    return;

                var radius = Menu.Item(Name + "DrawingRadius").GetValue<Slider>().Value;
                var color = Menu.Item(Name + "DrawingColor").GetValue<Color>();
                var thickness = Menu.Item(Name + "DrawingCircleThickness").GetValue<Slider>().Value;

                foreach (var spot in _wardSpots)
                {
                    if (spot.MagneticPosition.IsOnScreen())
                    {
                        Render.Circle.DrawCircle(spot.WardPosition, radius, color, 1);
                        if (spot.SafeSpot)
                        {
                            Render.Circle.DrawCircle(spot.MagneticPosition, radius, Color.White, thickness);
                            Drawing.DrawLine(Drawing.WorldToScreen(spot.MagneticPosition), Drawing.WorldToScreen(spot.WardPosition), 2f, color);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private void OnSpellbookCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            try
            {
                if (sender == null || args == null || !sender.Owner.IsMe || !Menu.Item(Name + "Hotkey").IsActive() || !IsWardSlot(args.Slot))
                    return;

                var spot = GetNearestWardSpot(Game.CursorPos);
                if (spot.Equals(default(WardSpot)) || spot.ClickPosition.Equals(args.EndPosition))
                    return;

                if (spot.SafeSpot)
                {
                    if (Game.CursorPos.Distance(spot.MagneticPosition) <= Menu.Item(Name + "DrawingRadius").GetValue<Slider>().Value)
                    {
                        args.Process = false;
                        if (_lastWardSpot.Equals(default(WardSpot)))
                        {
                            _lastWardSpot = spot;
                            _lastWardSlot = args.Slot;
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, spot.MovePosition);
                        }
                    }
                }
                else
                {
                    if (Game.CursorPos.Distance(spot.MagneticPosition) <= Menu.Item(Name + "DrawingRadius").GetValue<Slider>().Value)
                    {
                        args.Process = false;
                        sender.CastSpell(args.Slot, spot.ClickPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private void OnObjAiBaseNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (!sender.IsMe || _lastWardSpot.Equals(default(WardSpot)))
                return;

            if (!args.Path.Last().Equals(_lastWardSpot.MovePosition))
                _lastWardSpot = default(WardSpot);
        }

        private bool IsWardSlot(SpellSlot slot)
        {
            var iSlot = ObjectManager.Player.InventoryItems.FirstOrDefault(i => i.SpellSlot == slot);
            return _greenWards.Any(v => iSlot != null && (ItemId) v == iSlot.Id) || _pinkWards.Any(v => iSlot != null && (ItemId) v == iSlot.Id);
        }

        private WardSpot GetNearestWardSpot(Vector3 pos)
        {
            var nearest = float.MaxValue;
            var nSpot = default(WardSpot);
            foreach (var spot in _wardSpots)
            {
                var distance = Vector3.Distance(pos, spot.MagneticPosition);
                if (nearest > distance || nearest == float.MaxValue)
                {
                    nearest = distance;
                    nSpot = spot;
                }
            }
            return nSpot;
        }

        private void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (_lastCheck + CheckInterval > Environment.TickCount)
                    return;
                _lastCheck = Environment.TickCount;

                if (ObjectManager.Player.IsDead || _lastWardSpot.Equals(default(WardSpot)))
                    return;

                if (ObjectManager.Player.Position.Distance(_lastWardSpot.MovePosition) <= 3f)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_lastWardSlot, _lastWardSpot.ClickPosition);
                    _lastWardSpot = default(WardSpot);
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        private struct WardSpot
        {
            public readonly Vector3 ClickPosition;
            public readonly Vector3 MagneticPosition;
            public readonly Vector3 MovePosition;
            public readonly bool SafeSpot;
            public readonly Vector3 WardPosition;

            public WardSpot(Vector3 magneticPosition, Vector3 clickPosition, Vector3 wardPosition, Vector3 movePosition, bool safeSpot)
            {
                MagneticPosition = magneticPosition;
                ClickPosition = clickPosition;
                WardPosition = wardPosition;
                MovePosition = movePosition;
                SafeSpot = safeSpot;
            }

            public WardSpot(Vector3 magneticPosition)
            {
                MagneticPosition = magneticPosition;
                ClickPosition = magneticPosition;
                WardPosition = magneticPosition;
                MovePosition = magneticPosition;
                SafeSpot = false;
            }
        }
    }
}