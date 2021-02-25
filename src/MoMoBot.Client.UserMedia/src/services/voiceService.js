import config from "../config";
import request from "../utils/request";
import xmlbuilder from 'xmlbuilder';
const sdk = require('microsoft-cognitiveservices-speech-sdk');

const subscriptionKey = "65789dd635a8415bbf7d7d91d0a86b76";
const serviceRegion = "eastasia";

// now create the audio-config pointing to our stream and
// the speech config specifying the language.

const speechConfig = sdk.SpeechConfig.fromSubscription(subscriptionKey, serviceRegion);

// setting the recognition language to English.
speechConfig.speechRecognitionLanguage = "zh-CN";


export const upload = (blob, duration) => {
    let formData = new FormData();
    formData.append("file", blob);
    formData.append("duration", duration)
    return request({
        url: `${config.serverUrl}/api/voice/upload`,
        method: 'POST',
        data: formData
    });
}

export const convertToText = (blob, callback) => {
    var fileReader = new FileReader();
    let pushStream = null;
    fileReader.onload = (event) => {
        pushStream = sdk.AudioInputStream.createPushStream();
        const audioConfig = sdk.AudioConfig.fromStreamInput(pushStream);
        // create the speech recognizer.
        const recognizer = new sdk.SpeechRecognizer(speechConfig, audioConfig);
        const arrayBuffer = event.target.result;
        pushStream.write(arrayBuffer);
        recognizer.recognizeOnceAsync((result) => {
            const { text = "没有听清" } = result;
            recognizer.close();
            callback(text);
            // recognizer = undefined;
        }, (err) => {
            console.error("err - " + err);
            recognizer.close();
            // recognizer = undefined;
            callback('没有听清');
        });
    };
    fileReader.onloadend = () => {
        pushStream !== null && pushStream.close();
    }
    fileReader.readAsArrayBuffer(blob);
}

export const convertToVoice = (text,True) => {
    // let options = {
    //     method: 'POST',
    //     url: `https://${serviceRegion}.api.cognitive.microsoft.com/sts/v1.0/issueToken`,
    //     headers: {
    //         'Ocp-Apim-Subscription-Key': subscriptionKey
    //     }
    // };
    // return request(options)
    //     .then(accessToken => {
    //         let xml_body = `<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='zh-CN'>
    //         <voice name='Microsoft Server Speech Text to Speech Voice (zh-CN, Kangkang, Apollo)'>"${text}"</voice></speak>`
    //         // Convert the XML into a string to send in the TTS request.
    //         let body = xml_body.toString();
    //         request({
    //             method: 'POST',
    //             baseUrl: '',
    //             url: `https://${serviceRegion}.tts.speech.microsoft.com/cognitiveservices/v1`,
    //             headers: {
    //                 'Authorization': 'Bearer ' + accessToken,
    //                 'cache-control': 'no-cache',
    //                 // 'User-Agent': 'Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1',
    //                 'X-Microsoft-OutputFormat': 'riff-24khz-16bit-mono-pcm',
    //                 'Content-Type': 'application/ssml+xml'
    //             },
    //             data : body
    //         }).then(response=>{
    //             True(response);
    //         })
    //     })
}

export const getVoiceFile = (id) => {
    return request({
        url: `${config.serverUrl}/api/voice/${id}`,
        method: 'GET'
    });
}