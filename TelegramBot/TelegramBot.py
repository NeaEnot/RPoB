import configparser
import datetime
import os
import telebot
from telebot import types


config = configparser.ConfigParser()
config.read("config.ini")


bot = telebot.TeleBot(config.get("settings", "token"))


def log(user_id, message):
    with open(config.get("settings", "logPath"), 'a', encoding='cp1251') as log:
        log.write(f"{datetime.datetime.now()} - {message} (user: {user_id})\n")


@bot.message_handler(content_types=['text'])
def get_text_messages(message):
    if message.text == "/part":
        part_text = ""

        with open(config.get("settings", "outPath"), 'r', encoding='utf8') as file:
            for line in file:
                if len(part_text) + len(line) < 5000:
                    part_text += line + "\n"
                else:
                    break

        keyboard = types.InlineKeyboardMarkup()
        key_mistake = types.InlineKeyboardButton(text='В тексте ошибка', callback_data='mistake')
        keyboard.add(key_mistake)

        bot.send_message(message.from_user.id, text=part_text, reply_markup=keyboard)
        log(message.from_user.id, "Отправлен отрывок")

        os.startfile(config.get("settings", "applicationPath"))
    elif message.text == "/catalog":
        with open(config.get("settings", "catalogPath"), 'r', encoding='utf8') as file:
            catalog = ""
            for line in file:
                if len(catalog) + len(line) < 5000:
                    catalog += line
                else:
                    bot.send_message(message.from_user.id, text=catalog)
                    catalog = line
            if len(catalog) > 0:
                bot.send_message(message.from_user.id, text=catalog)
    elif message.text == "/help":
        bot.send_message(message.from_user.id, "Бот в ответ на запрос отправляет отрывок из случайным образом выбранной книги из библиотеки.\n" \
                                                "/help - получение справки;\n/part - получение отрывка\n/catalog - получение каталога книг, находящихся в библиотеке.")
    else:
        bot.send_message(message.from_user.id, "Неизвестная команда. Напишите /help.")


@bot.message_handler(content_types=['document', 'audio'])
def get_other_document_messages(message):
    bot.send_message(message.from_user.id, "Неизвестная команда. Напишите /help.")


@bot.callback_query_handler(func=lambda call: True)
def callback_worker(call):
    if call.data == "mistake":
        log(call.from_user.id, "Пользователь сообщил об ошибке")
        bot.send_message(call.message.chat.id, "Наличие ошибки было зафиксировано в файле логов. Ошибка будт исправлена в ближайшее время.")
    else:
        bot.send_message(call.message.from_user.id, "Неизвестная команда. Напишите /help.")


bot.polling()