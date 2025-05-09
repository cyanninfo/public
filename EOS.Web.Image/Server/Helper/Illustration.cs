using System.Drawing;

namespace EOS.Web.Image.Server.Helper
{
    public class Illustration
    {
        const string CheminRelatifCalqueGauche = @"Calque\Calque gauche.png";
        const string CheminRelatifCalqueLogo = @"Calque\Calque logo.png";

        static Size TailleCible = new(1500, 1024);
        static readonly int MargeDroiteLogo = 40;
        static readonly int MargeBasLogo = 40;
        static readonly int MargeurZoneTexteMax = 450;
        static readonly int MargeBasZoneTexte = 40;

        static readonly Color CouleurTexte = Color.White;
        static readonly Color CouleurOmbre = Color.FromArgb(0, 73, 196);
        private readonly Bitmap bitmap0;
        private readonly IWebHostEnvironment _env;

        public Illustration(Bitmap bitmap, IWebHostEnvironment env)
        {
            this.bitmap0 = bitmap;
            _env = env;
        }

        public Bitmap Formate()
        {
            using Bitmap image1 = Dimensionne(bitmap0);
            using Bitmap image2 = CalqueGauche(image1);
            Bitmap image3 = LogoDroite(image2);

            return image3;
        }

        public Bitmap Formate(string texte)
        {
            using Bitmap image3 = Formate();
            Bitmap image4 = Ecris(image3, texte);
            return image4;
        }

        private Bitmap Dimensionne(Bitmap source)
        {
            int targetWidth = (int)(source.Width * (TailleCible.Height / (float)source.Height));

            // Créer une nouvelle image redimensionnée
            Bitmap resizedImage = new Bitmap(targetWidth, TailleCible.Height);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                // Configuration pour un rendu de qualité
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Dessiner l'image redimensionnée
                graphics.DrawImage(source, 0, 0, targetWidth, TailleCible.Height);
            }

            // Sauvegarder l'image redimensionnée
            return resizedImage;
        }

        private Bitmap CalqueGauche(Bitmap source)
        {
            Size tailleCalculee = new Size(TailleCible.Width, source.Height);

            Bitmap imageResultat = new Bitmap(tailleCalculee.Width, tailleCalculee.Height);

            using (Graphics graphic = Graphics.FromImage(imageResultat))
            {
                graphic.Clear(Color.Transparent);
            }

            // Dessine illustration à droite
            using (Bitmap illustration = new Bitmap(source))
            {
                using (Graphics graphic = Graphics.FromImage(imageResultat))
                {
                    int x = TailleCible.Width - illustration.Width;
                    int y = 0;
                    graphic.DrawImage(illustration, x, y);
                }
            }

            // Dessine calque à gauche
            using (Bitmap calque = new Bitmap(Path.Combine(_env.ContentRootPath, CheminRelatifCalqueGauche)))
            {
                using (Graphics graphic = Graphics.FromImage(imageResultat))
                {
                    int x = 0;
                    int y = 0;
                    graphic.DrawImage(calque, x, y, tailleCalculee.Width, tailleCalculee.Height);
                }
            }

            return imageResultat;
        }

        private Bitmap LogoDroite(Bitmap source)
        {
            Bitmap imageResultat = new Bitmap(source);

            using (Graphics graphic = Graphics.FromImage(imageResultat))
            {
                using (Bitmap logo = new Bitmap(Path.Combine(_env.ContentRootPath, CheminRelatifCalqueLogo)))
                {
                    int xLogo = imageResultat.Width - logo.Width - MargeDroiteLogo;
                    int yLogo = imageResultat.Height - logo.Height - MargeBasLogo;
                    graphic.DrawImage(logo, xLogo, yLogo, logo.Width, logo.Height);
                }
            }
            return imageResultat;
        }

        private Bitmap Ecris(Bitmap source, string texte)
        {
            if (!string.IsNullOrWhiteSpace(texte))
            {
                Bitmap imageResultat = new Bitmap(source);
                using (Graphics graphic = Graphics.FromImage(imageResultat))
                {
                    using (Font font = new Font("Arial", 30, FontStyle.Italic))
                    {
                        SizeF tailleZoneTexteCalcule = CalculRectangleTexte(texte, font, MargeurZoneTexteMax);
                        int zoneTexteX = MargeurZoneTexteMax / 2 - (int)tailleZoneTexteCalcule.Width / 2;
                        int zoneTexteY = imageResultat.Height - (int)tailleZoneTexteCalcule.Height - MargeBasZoneTexte;

                        using (Brush brushTexte = new SolidBrush(CouleurTexte))
                        {
                            using (Brush brushOmbre = new SolidBrush(CouleurOmbre))
                            {
                                using (StringFormat stringFormat = new StringFormat())
                                {
                                    graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                    stringFormat.Alignment = StringAlignment.Near;
                                    stringFormat.LineAlignment = StringAlignment.Near;
                                    graphic.DrawString(texte, font, brushOmbre, new Rectangle(zoneTexteX - 4, zoneTexteY + 4, (int)tailleZoneTexteCalcule.Width + 10, (int)tailleZoneTexteCalcule.Height + 10), stringFormat);
                                    graphic.DrawString(texte, font, brushTexte, new Rectangle(zoneTexteX, zoneTexteY, (int)tailleZoneTexteCalcule.Width + 10, (int)tailleZoneTexteCalcule.Height + 10), stringFormat);
                                }
                            }
                        }
                    }

                    return imageResultat;
                }
            }
            else
                return source;
        }

        static SizeF CalculRectangleTexte(string text, Font font, float maxWidth)
        {
            using (Bitmap tempBitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(tempBitmap))
            {
                // Configurer le mode de rendu pour des mesures précises
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // Calculer la taille nécessaire
                StringFormat stringFormat = new StringFormat
                {
                    FormatFlags = StringFormatFlags.LineLimit // Limiter à la largeur maximale
                };

                return graphics.MeasureString(text, font, (int)maxWidth, stringFormat);
            }
        }
    }
}
