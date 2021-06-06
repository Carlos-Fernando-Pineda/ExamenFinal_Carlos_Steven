using Progra1Bot.Clases.Base_de_datos;
using Progra1Bot.Clases.emojis;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Progra1Bot.Clases
{
    public class clsEjemplo2
    {
        private static TelegramBotClient Bot;

        public async Task IniciarTelegram()
        {
            Bot = new TelegramBotClient("1883821855:AAFgxNl1U1hQVoaQIG1e034Pyr36uOUnp54");

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            Bot.OnMessage += BotCuandoRecibeMensajes;
            Bot.OnMessageEdited += BotCuandoRecibeMensajes;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"escuchando solicitudes del BOT @{me.Username}");



            Console.ReadLine();
            Bot.StopReceiving();
        }

        // cuando recibe mensajes
        private static async void BotCuandoRecibeMensajes(object sender, MessageEventArgs messageEventArgumentos)
        {
            var ObjetoMensajeTelegram = messageEventArgumentos;
            var mensajes = ObjetoMensajeTelegram.Message;

            string mensajeEntrante = mensajes.Text;


            string respuesta = "Escribe 'ayuda' para mas opciones";
            if (mensajes == null || mensajes.Type != MessageType.Text)
                return;

            Console.WriteLine($"Recibiendo Mensaje del chat {ObjetoMensajeTelegram.Message.Chat.Id}.");
            Console.WriteLine($"Dice {ObjetoMensajeTelegram.Message.Text}.");


            //tolower
            if (mensajes.Text.ToLower().Contains("hola"))
            {
                respuesta = "Hola me da mucho gusto de Saludarte!!!";
            }

            if (mensajes.Text.ToLower().Contains("hora"))
            {
                DateTime fecha = DateTime.Now;
                respuesta = "Pues, ahorita son las " + fecha.Hour + " con " + fecha.Minute;
            }
            if (mensajes.Text.ToLower().Contains("menu"))
            {
                respuesta = "Selecciona una opción:";
                switch (mensajes.Text.Split(' ').First())
                {
                    case "/inline":
                        await SendInlineKeyboard(mensajes);
                        break;
                    case "/keyboard":
                        await SendReplyKeyboard(mensajes);
                        break;
                    case "/request":
                        await RequestContactAndLocation(mensajes);
                        break;
                    default:
                        await Usage(mensajes);
                        break;
                }
            }
            if (mensajes.Text.ToLower().Contains("ayuda"))
            {
                respuesta = "Los comandos registrados son:\n" +
                            "Hola\n" +
                            "Hora\n" +
                            "Db\n" +
                            "Menu";
            }
            if (mensajes.Text.ToLower().Contains("db"))
            {
                BasedeDatos();
            }

            if (!String.IsNullOrEmpty(respuesta))//    
            {
                await Bot.SendTextMessageAsync(
                    chatId: ObjetoMensajeTelegram.Message.Chat,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    text: respuesta

            );
            }

        } // fin del metodo de recepcion de mensajes

        public static void BasedeDatos()
        {
            ClsConexion cn = new ClsConexion();
            cn.consultaTablaDirecta($"insert into [tb_nombres] values(0905207600,'Carlos','Pineda')");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("UPS!!! Recibo un error!!!: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }

        static async Task Usage(Message mensajes)
        {
            const string usage = "/inline   - Escoge un emoji, revelará un pasaje\n" +
                                 "/keyboard - Escoge un emoji";
            await Bot.SendTextMessageAsync(
                chatId: mensajes.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        static async Task SendInlineKeyboard(Message mensajes)
        {
            await Bot.SendChatActionAsync(mensajes.Chat.Id, ChatAction.Typing);

            // Simulate longer running task
            await Task.Delay(500);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(mdEmoji.smile, "Hoy es un nuevo día. Incluso si lo hiciste mal ayer, hoy lo puedes hacer bien."),
                        InlineKeyboardButton.WithCallbackData(mdEmoji.pulgarAbajo, "El fracaso es una buena oportunidad para empezar de nuevo con más inteligencia."),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(mdEmoji.nerd, "Sueña como si fueras a vivir para siempre, vive como si fueses a morir hoy."),
                        InlineKeyboardButton.WithCallbackData(mdEmoji.carapensante, "Hacen falta días malos para darte cuenta de lo bonitos que son el resto."),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(mdEmoji.correo,"Si quieres que tus sueños se hagan realidad, el primer paso es levantarte."),
                        InlineKeyboardButton.WithCallbackData(mdEmoji.caralentesoscuros,"Todos tus sueños pueden hacerse realidad si tienes el coraje de perseguirlos."),
                    }
                });
            await Bot.SendTextMessageAsync(
                chatId: mensajes.Chat.Id,
                text: "Elige",
                replyMarkup: inlineKeyboard
            );
        }

        static async Task SendReplyKeyboard(Message mensajes)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                        new KeyboardButton[] { mdEmoji.Rewind, mdEmoji.Watch },
                        new KeyboardButton[] { mdEmoji.sorprendido, mdEmoji.telefono },
                },
                resizeKeyboard: true
            );

            await Bot.SendTextMessageAsync(
                chatId: mensajes.Chat.Id,
                text: "Elige",
                replyMarkup: replyKeyboardMarkup

            );
        }

        static async Task RequestContactAndLocation(Message message)
        {
            var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });
            await Bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Dónde estás tu?",
                replyMarkup: RequestReplyKeyboard
            );

        }
    }
}
