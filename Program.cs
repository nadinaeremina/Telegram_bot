using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExamTGBot
{
    internal class Program
    {
        // клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и т.д.
        private static ITelegramBotClient _botClient;

        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions _receiverOptions;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("7657815829:AAFDrZP13rpppM37yMvcgy8uFAbAr7Icu8o"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
            _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
            {
                AllowedUpdates = new[] // Типы получаемых Update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                UpdateType.CallbackQuery // Inline кнопки
            },
                // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
                // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
                ThrowPendingUpdates = false
            };
            using var cts = new CancellationTokenSource();
            // UpdateHander - обработчик приходящих Update`ов
            // ErrorHandler - обработчик ошибок, связанных с Bot API
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

            var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            // Эта переменная будет содержать в себе все связанное с сообщениями
                            var message = update.Message;

                            // From - это от кого пришло сообщение (или любой другой Update)
                            var user = message.From;

                            // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            // Chat - содержит всю информацию о чате
                            var chat = message.Chat;

                            // Добавляем проверку на тип Message
                            switch (message.Type)
                            {
                                // Текстовый тип
                                case MessageType.Text:
                                    {
                                        // тут обрабатываем команду /start
                                        if (message.Text == "/start")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Привет! Я ваш бот-помощник команды Otdohny.ru!\n " +
                                                "Выберите то, что вас интересует:\n" +
                                                "Перейти в Whatsapp, чтобы поговорить с живым помощником\n" +
                                                "/whatsapp\n" +
                                                "Задать вопрос\n" +
                                                "/faq\n");
                                            return;
                                        }

                                        if (message.Text == "/whatsapp")
                                        {
                                            // Тут создаем нашу клавиатуру
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                                {
                                        // Каждый новый массив - это дополнительные строки,
                                        // а каждая дополнительная кнопка в массиве - это добавление ряда

                                        new InlineKeyboardButton[] // тут создаем массив кнопок
                                        {
                                            InlineKeyboardButton.WithUrl("Перейти в Whatsapp", "https://wa.me/+79039127706"),
                                            
                                        }
                                                });

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Это меню для перехода в чат в Whatsapp!",
                                                replyMarkup: inlineKeyboard); // Все клавиатуры передаются в параметр replyMarkup

                                            return;
                                        }

                                        if (message.Text == "/faq")
                                        {
                                            // Тут все аналогично Inline клавиатуре, только меняются классы
                                            // НО! Тут потребуется дополнительно указать один параметр, чтобы
                                            // клавиатура выглядела нормально, а не как попало

                                            var replyKeyboard = new ReplyKeyboardMarkup(
                                                new List<KeyboardButton[]>()
                                                {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Для кого это приложение?")
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Как сдать жильё?")
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Как снять жильё?")
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("В каких городах будет работать ваше приложение?")
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Чем вы лучше других приложений?")
                                        }
                                                })
                                            {
                                                // автоматическое изменение размера клавиатуры, если не стоит true,
                                                // тогда клавиатура растягивается сильно
                                                ResizeKeyboard = true,
                                            };

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Это FAQ клавиатура!",
                                                replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

                                            return;
                                        }

                                        if (message.Text == "Для кого это приложение?")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Это приложение создано для всех желающих выгодно снять или сдать жильё: " +
                                                "дома, квартиры, апартаменты, базы отдыха и многое другое! " +
                                                "Приложение поможет вам найти самый подходящий вариант для отдыха, досуга или командировки, " +
                                                "если вы арендатор. Или поможет найти самых лучших аренадторов для своих помещений, " +
                                                "если вы арендодатель. Обязательно регистриуйтесь и пользуйтесь всеми услугами нашего приложения!",
                                                replyToMessageId: message.MessageId);

                                            return;
                                        }

                                        if (message.Text == "Как сдать жильё?")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Чтобы сдать жильё сначала необходимо пройти несложную регистрацию. " +
                                                "Нам нужны ваши ФИО, фотография и данные о количестве объектов. " +
                                                "Не забудьте придумать логин и пароль! После завершения регистрации Вам будет доступна " +
                                                "возможность создания объявлений!",
                                                replyToMessageId: message.MessageId);

                                            return;
                                        }
                                        if (message.Text == "Как снять жильё?")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Чтобы снять жильё сначала необходимо пройти несложную регистрацию. " +
                                                "Нам нужны ваши ФИО, фотография и дата рождения." +
                                                "Не забудьте придумать логин и пароль! После завершения регистрации Вам будет доступна " +
                                                "возможность оформления брони! Просматривать объявления вы сможете без регистрации.",
                                                replyToMessageId: message.MessageId);

                                            return;
                                        }

                                        if (message.Text == "В каких городах будет работать ваше приложение?")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "В настоящий момент приложение разработано только для трёх городов: Калининград, Екатеринбург и Иркутск!" +
                                                "Но мы обещаем, что вскоре расширим свою географию и будем радовать Вас " +
                                                "доступным жильём во всех уголках нашей необъятной Родины!",
                                                replyToMessageId: message.MessageId);

                                            return;
                                        }
                                        if (message.Text == "Чем вы лучше других приложений?")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Мы используем систему фиксированной комиссии, благодаря чему наши арендодатели не переплачивают за опубликованные объявления! " +
                                                "Мы хотим сделать систему аренды жилья простой и доступной для всех!",
                                                replyToMessageId: message.MessageId);

                                            return;
                                        }

                                        return;
                                    }


                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используйте только текст!");
                                        return;
                                    }
                            }

                            return;
                        }

                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

}
