import gnupg
import os
import smtplib
from email.mime.text import MIMEText

def send_encrypted_message(gpg, sender_email, sender_password, recipient_email, message):
    fingerprint = 'D388FA1A7DC0ED1C212500B765BF2E115C5D0A50'  # Здесь нужно указать отпечаток ключа получателя
    ciphertext = str(gpg.encrypt(message, fingerprint, always_trust=True))

    msg = MIMEText(ciphertext)
    msg['Subject'] = 'Subject'
    msg['From'] = sender_email
    msg['To'] = recipient_email

    with smtplib.SMTP('mail.syneforge.com', 587) as server:
        server.starttls()
        server.login(sender_email, sender_password)
        server.sendmail(sender_email, recipient_email, msg.as_string())

gpg = gnupg.GPG(gnupghome="C:\\Users\\ladmael\\Documents\\gpg")

with open('./syneforge.pgp', 'rb') as f:
    imported_keys = gpg.import_keys(f.read())
    print(imported_keys.results)

plaintext = "Hello, World!"
for key in imported_keys.results:
    fingerprint = key['fingerprint']
    email = gpg.get_key(fingerprint).uids[0].split()[0].strip('<>')

    send_encrypted_message(gpg, 'ladmael@syneforge.com', 'pass', email, plaintext)
