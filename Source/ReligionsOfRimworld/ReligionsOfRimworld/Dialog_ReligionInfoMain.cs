﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ReligionsOfRimworld
{
    class Dialog_ReligionInfoMain : Window
    {
        Religion religion;
        float currentY = 0;
        float padding = 50;
        private static float listHeight;
        private static Vector2 scrollPosition;
        private Dialog_InfoCard.InfoCardTab tab;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1150f, 900f);
            }
        }

        protected override float Margin
        {
            get
            {
                return 0.0f;
            }
        }

        public Dialog_ReligionInfoMain(Religion religion)
        {
            this.religion = religion;
            this.forcePause = true;
            scrollPosition = new Vector2();
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
            this.soundAppear = SoundDefOf.InfoCard_Open;
            this.soundClose = SoundDefOf.InfoCard_Close;
        }

        private void Template(Rect inner, string header, int rows, int columns, IEnumerable<Action<Rect>> actions)
        {
            Rect labelRect = new Rect(inner);
            labelRect.height = Verse.Text.CalcHeight(header, inner.width);
            Widgets.Label(labelRect, header);

            Rect toAction = inner.ContractedBy(20);

            Template(inner.ContractedBy(20), rows, columns, actions);
        }

        private void Template(Rect inner, int rows, int columns, IEnumerable<Action<Rect>> actions)
        {
            Vector2 size = new Vector2(inner.width / columns, inner.height / rows);
            float curX = inner.x;
            float curY = inner.y;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    actions.ElementAt(columns * i + j)(new Rect(new Vector2(curX, curY), size));
                    curX += size.x;
                }
                curY += size.y;
                curX = inner.x;
            }
        }

        private string GetRange(PietyDef def)
        {
            return $"{def.Stages.First().PietyOffset} to {def.Stages.Last().PietyOffset}";
        }

        private string GetRange(ThoughtDef def)
        {
            return $"{def.stages.First().baseMoodEffect} to {def.stages.Last().baseMoodEffect}";
        }

        public override void DoWindowContents(Rect inRect)
        {

            List<TabRecord> tabs = new List<TabRecord>();
            TabRecord item = new TabRecord("Main".Translate(), delegate ()
            {
                this.tab = Dialog_InfoCard.InfoCardTab.Stats;
            }, this.tab == Dialog_InfoCard.InfoCardTab.Stats);
            TabRecord item2 = new TabRecord("Social".Translate(), delegate ()
            {
                this.tab = Dialog_InfoCard.InfoCardTab.Records;
            }, this.tab == Dialog_InfoCard.InfoCardTab.Records);
            tabs.Add(item);
            tabs.Add(item2);


            Rect tabRect = inRect.ContractedBy(18f);
            tabRect.yMin += 45f;
            TabDrawer.DrawTabs(tabRect, tabs, 200f);

            Fill(tabRect.ContractedBy(18));
        }

        private void Fill(Rect inner)
        {
            if (tab == Dialog_InfoCard.InfoCardTab.Stats)
            {
                Rect tophalf = inner.TopHalf();
                Vector2 topHalfSize = tophalf.size / new Vector2(1, 3);

                Rect description = new Rect(tophalf.position, topHalfSize);
                Rect doctrines = new Rect(tophalf.position + new Vector2(0, topHalfSize.y), topHalfSize);
                Rect buildings = new Rect(tophalf.position + new Vector2(0, topHalfSize.y) * 2, topHalfSize);

                Rect bottomHalf = inner.BottomHalf();

                Rect criteriaRect = bottomHalf.LeftHalf();
                Rect opinionRect = bottomHalf.RightHalf();

                Template(description, religion.Label, 1, 1, new Action<Rect>[1] { x => Widgets.Label(x, religion.Description) });

                Template(doctrines, "Doctrines", 2, 3, Doctrines());
                Template(buildings, "Buildings", 1, Buildings().Count(), Buildings());
                Template(criteriaRect, "Criteria", Criteria().Count(), 1, Criteria());
                Template(opinionRect, "Opinion", Opinion().Count(), 1, Opinion());
            }
            else if (tab == Dialog_InfoCard.InfoCardTab.Records)
            {
                Template(inner, "Social", 1, 1, SocialSettingsHeader());
            }
        }

        private IEnumerable<Action<Rect>> Buildings()
        {
            ReligionSettings_AllowedBuildings settings = religion.GetSettings<ReligionSettings_AllowedBuildings>(SettingsTagDefOf.AllowedBuildingsTag);
            if (settings == null)
            {
                yield return x => Widgets.Label(x, "No buildings");
            }
            else
            {
                foreach (var building in settings.AllowedBuildings)
                {
                    yield return x =>
                    {
                        Widgets.DrawTextureFitted(x, building.uiIcon, 1);
                    };
                }
            }
        }

        private IEnumerable<Action<Rect>> Criteria()
        {
            ReligionSettings_JoiningCriteria jSettings = religion.GetSettings<ReligionSettings_JoiningCriteria>(SettingsTagDefOf.JoiningCriteriaTag);
            if (jSettings == null)
            {
                yield return x => Widgets.Label(x, "No criteria");
            }
            else
            {
                foreach (var criteria in jSettings.Criteria.OrderByDescending(o => o.Importance))
                {
                    yield return x =>
                    {
                        Template(x, 1, 3, new Action<Rect>[3]
                        {
                            u => Widgets.DrawTextureFitted(u, criteria.ShouldHave ? GraphicsCache.CheckboxOnTex : GraphicsCache.CheckboxOffTex, 1),
                            u =>
                            {
                                GUI.color = Color.gray;
                                Widgets.Label(u, criteria.Importance.ToString());
                                GUI.color = Color.white;
                            },
                            u => Widgets.Label(u, criteria.Reason)
                        });
                    };
                }
            }
        }

        private IEnumerable<Action<Rect>> Opinion()
        {
            ReligionSettings_Social oSettings = religion.GetSettings<ReligionSettings_Social>(SettingsTagDefOf.OpinionTag);
            if (oSettings == null)
            {
                yield return x => Widgets.Label(x, "No opinion");
            }
            else
            {
                if (oSettings.DefaultPropety != null)
                {
                    yield return x =>
                    {
                        var op = oSettings.DefaultPropety.Witness;
                        Template(x, 1, 3, new Action<Rect>[3]
                        {
                            u =>
                            {
                                GUI.color = op == null ? Color.white : op.OpinionThought.stages[0].baseOpinionOffset > 0 ? Color.green : Color.red;
                                Widgets.Label(u, op == null ? "0" : $"{op.OpinionThought.stages[0].baseOpinionOffset} to {op.OpinionThought.stages.Last().baseOpinionOffset}");
                                GUI.color = Color.white;
                            },
                            u =>
                            {
                                GUI.color = Color.gray;
                                Widgets.Label(u, oSettings.DefaultPropety.PawnCategory.ToString());
                                GUI.color = Color.white;
                            },
                            u => Widgets.Label(u, "Everyone")
                        });
                    };
                }
                foreach (var opinion in oSettings.Properties)
                {
                    yield return x =>
                    {
                        var op = opinion.Witness;
                        Template(x, 1, 3, new Action<Rect>[3]
                        {
                            u =>
                            {
                                GUI.color = op == null ? Color.white : op.OpinionThought.stages[0].baseOpinionOffset > 0 ? Color.green : Color.red;
                                Widgets.Label(u, op == null ? "0" : $"{op.OpinionThought.stages[0].baseOpinionOffset} to {op.OpinionThought.stages.Last().baseOpinionOffset}");
                                GUI.color = Color.white;
                            },
                            u =>
                            {
                                GUI.color = Color.gray;
                                Widgets.Label(u, opinion.PawnCategory.ToString());
                                GUI.color = Color.white;
                            },
                            u => Widgets.Label(u, opinion.GetObject().LabelCap)
                        });
                    };
                }
            }
        }

        private IEnumerable<Action<Rect>> Doctrines()
        {
            List<Tuple<string, bool>> settings = new List<Tuple<string, bool>>();
            settings.Add(new Tuple<string, bool>("Have piety", religion.GetSettings(SettingsTagDefOf.NeedTag) != null));
            settings.Add(new Tuple<string, bool>("Can converts through talks", religion.GetSettings(SettingsTagDefOf.TalksTag) != null));
            settings.Add(new Tuple<string, bool>("Can converts through incidents", religion.GetSettings(SettingsTagDefOf.IncidentsTag) != null));
            settings.Add(new Tuple<string, bool>("Can converts through breakdowns", religion.GetSettings(SettingsTagDefOf.MentalBreaksTag) != null));
            settings.Add(new Tuple<string, bool>("Can pray", religion.GetSettings(SettingsTagDefOf.PrayingsTag) != null));
            settings.Add(new Tuple<string, bool>("Have actitvites", religion.GetSettings(SettingsTagDefOf.ActivityTag) != null));

            foreach (var s in settings)
            {
                yield return x =>
                {
                    Vector2 size = x.size / 2;
                    Widgets.Label(new Rect(x.x, x.y, size.x, size.y), s.Item1);
                    GUI.color = s.Item2 ? Color.green : Color.red;
                    Widgets.Label(new Rect(x.x, x.y + size.y, size.x, size.y), s.Item2.ToString());
                    GUI.color = Color.white;
                };
            }
        }

        private IEnumerable<Action<Rect>> SocialSettingsHeader()
        {
            IEnumerable<ReligionSettings> sSettings = religion.AllSettings.Where(g => g is ReligionSettings_Social);
            if (sSettings == null)
            {
                yield return x => Widgets.Label(x, "No social");
            }
            else
            {
                yield return z =>
                {
                    Template(z, 1, 7, new Action<Rect>[7]
                    {
                                u => Widgets.Label(u, "cause"),
                                u => Widgets.Label(u, "settings"),
                                u => Widgets.Label(u, "category"),
                                u => Widgets.Label(u, "subjectPiety"),
                                u => Widgets.Label(u, "subjectThought"),
                                u => Widgets.Label(u, "witnessPiety"),
                                u => Widgets.Label(u, "witnessThought"),
                    });
                    Rect outR = new Rect(z.x, z.y + 50, z.width, z.height);
                    Rect viewR = new Rect(z.x, z.y + 50, z.width - 16, z.height - 50);
                    Widgets.BeginScrollView(outR, ref scrollPosition, viewR, true);
                    Template(viewR, SocialSettings().Concat(ActivitySettings()).Count(), 1, SocialSettings().Concat(ActivitySettings()));
                    Widgets.EndScrollView();
                };
            }
        }

        private IEnumerable<Action<Rect>> SocialSettings()
        {
            foreach (ReligionSettings_Social s in religion.AllSettings.Where(g => g is ReligionSettings_Social))
            {
                if (s.DefaultPropety != null)
                {
                    yield return z =>
                    {
                        Template(z, 1, 7, new Action<Rect>[7]
                        {
                                u => Widgets.Label(u, "Default"),
                                u => Widgets.Label(u, s.Tag.LabelCap),
                                u => Widgets.Label(u, s.DefaultPropety.PawnCategory.ToString()),
                                u => Widgets.Label(u, s.DefaultPropety.Subject?.Piety != null ? GetRange(s.DefaultPropety.Subject.Piety) : "-"),
                                u => Widgets.Label(u, s.DefaultPropety.Subject?.Thought != null ? GetRange(s.DefaultPropety.Subject.Thought) : "-"),
                                u => Widgets.Label(u, s.DefaultPropety.Witness?.Piety != null ? GetRange(s.DefaultPropety.Witness.Piety) : "-"),
                                u => Widgets.Label(u, s.DefaultPropety.Witness?.Thought != null ? GetRange(s.DefaultPropety.Witness.Thought) : "-"),
                        });
                    };
                }
                foreach (var p in s.Properties)
                {
                    yield return z =>
                    {
                        Template(z, 1, 7, new Action<Rect>[7]
                        {
                                u => Widgets.Label(u, p.GetObject().LabelCap),
                                u => Widgets.Label(u, s.Tag.LabelCap),
                                u => Widgets.Label(u, p.PawnCategory.ToString()),
                                u => Widgets.Label(u, p.Subject?.Piety != null ? GetRange(p.Subject.Piety) : "-"),
                                u => Widgets.Label(u, p.Subject?.Thought != null ? GetRange(p.Subject.Thought) : "-"),
                                u => Widgets.Label(u, p.Witness?.Piety != null ? GetRange(p.Witness.Piety) : "-"),
                                u => Widgets.Label(u, p.Witness?.Thought != null ? GetRange(p.Witness.Thought) : "-"),
                        });
                    };
                }
            }
        }

        private IEnumerable<Action<Rect>> ActivitySettings()
        {
            ReligionSettings_ReligionActivity aSettings = religion.GetSettings<ReligionSettings_ReligionActivity>(SettingsTagDefOf.ActivityTag);
            if (aSettings != null)
            {
                foreach (var p in aSettings.Properties)
                {
                    yield return z =>
                    {
                        Template(z, 1, 7, new Action<Rect>[7]
                        {
                                u => Widgets.Label(u, p.GetObject().LabelCap),
                                u => Widgets.Label(u, aSettings.Tag.LabelCap),
                                u => Widgets.Label(u, p.PawnCategory.ToString()),
                                u => Widgets.Label(u, p.Subject?.Piety != null ? GetRange(p.Subject.Piety) : "-"),
                                u => Widgets.Label(u, p.Subject?.Thought != null ? GetRange(p.Subject.Thought) : "-"),
                                u => Widgets.Label(u, p.Witness?.Piety != null ? GetRange(p.Witness.Piety) : "-"),
                                u => Widgets.Label(u, p.Witness?.Thought != null ? GetRange(p.Witness.Thought) : "-"),
                        });
                    };
                }
            }
        }
    }
}
