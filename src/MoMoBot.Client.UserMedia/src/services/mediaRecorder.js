export const MP3Recorder = (config) => {

    var recorder = this;
    config = config || {};
    config.sampleRate = config.sampleRate || 44100;
    config.bitRate = config.bitRate || 128;

    navigator.getUserMedia = navigator.getUserMedia ||
        navigator.webkitGetUserMedia ||
        navigator.mozGetUserMedia ||
        navigator.msGetUserMedia;

    if (navigator.getUserMedia) {
        navigator.getUserMedia({
            audio: true
        },
            function (stream) {
                var context = new AudioContext(),
                    microphone = context.createMediaStreamSource(stream),
                    processor = context.createScriptProcessor(16384, 1, 1),//bufferSize大小，输入channel数，输出channel数
                    mp3ReceiveSuccess, currentErrorCallback;

                config.sampleRate = context.sampleRate;
                var pauseContinue = true;
                processor.onaudioprocess = function (event) {
                    if (pauseContinue) {
                        //边录音边转换
                        var array = event.inputBuffer.getChannelData(0);
                        // console.log(array)
                        // console.log(array)
                        realTimeWorker.postMessage({ cmd: 'encode', buf: array });
                    }
                };
                var realTimeWorker = new Worker('mp3js/worker-realtime.js');
                realTimeWorker.onmessage = function (e) {
                    switch (e.data.cmd) {
                        case 'init':
                            log('初始化成功');
                            if (config.initOK) {
                                config.initOK();
                            }
                            break;
                        case 'end':
                            if (mp3ReceiveSuccess) {
                                var fileReader = new FileReader();
                                var blob = new Blob(e.data.buf, { type: 'audio/mp3' })
                                fileReader.onload = function () {
                                    var bufferArray = this.result;
                                    var blobUrl = URL.createObjectURL(blob);
                                    var audio = new Audio();                //重新创建一个新的audio对象，为了下面获取长度的时候避免每次都获取同一个audio的长度
                                    audio.src = blobUrl;　　　　　　　　　　 //重新设置新的audio对象的音频url　　
                                    audio.preload = "metadata";               //设置新的audio对象加载音频元数据
                                    audio.load();
                                    audio.onloadedmetadata = function () {  // 新的audio对象加载音频元数据完成时回调
                                        var second = audio.duration;    // 时长
                                        mp3ReceiveSuccess(encode64(bufferArray), blobUrl, second);
                                    }
                                }
                                fileReader.readAsArrayBuffer(blob);
                                // console.log(blob)
                            }
                            break;
                        case 'error':
                            log('错误信息：' + e.data.error);
                            if (currentErrorCallback) {
                                currentErrorCallback(e.data.error);
                            }
                            break;
                        default:
                            log('未知信息：', e.data);
                    }
                };

                recorder.getMp3Blob = function (onSuccess, onError) {
                    currentErrorCallback = onError;
                    mp3ReceiveSuccess = onSuccess;
                    realTimeWorker.postMessage({ cmd: 'finish' });
                };

                recorder.start = function () {
                    pauseContinue = true;
                    if (processor && microphone) {
                        microphone.connect(processor);
                        processor.connect(context.destination);
                        log('开始录音');
                    }
                }
                recorder.pauseRecord = function () {
                    pauseContinue = false;
                    // if (processor && microphone) {
                    //     microphone.connect(processor);
                    //     processor.connect(context.destination);
                    //     log('zanting');
                    // }
                }
                recorder.continueRcord = function () {
                    pauseContinue = true;
                    // if (processor && microphone) {
                    //     microphone.connect(processor);
                    //     processor.connect(context.destination);
                    //     log('jixu');
                    // }
                }

                recorder.stop = function () {
                    pauseContinue = false;
                    if (processor && microphone) {
                        microphone.disconnect();
                        processor.disconnect();
                        log('录音结束');
                        console.log('jieshu')
                    }
                }

                realTimeWorker.postMessage({
                    cmd: 'init',
                    config: {
                        sampleRate: config.sampleRate,
                        bitRate: config.bitRate
                    }
                });
            },
            function (error) {
                var msg;
                switch (error.code || error.name) {
                    case 'PERMISSION_DENIED':
                    case 'PermissionDeniedError':
                        msg = '用户拒绝访问麦客风';
                        break;
                    case 'NOT_SUPPORTED_ERROR':
                    case 'NotSupportedError':
                        msg = '浏览器不支持麦客风';
                        break;
                    case 'MANDATORY_UNSATISFIED_ERROR':
                    case 'MandatoryUnsatisfiedError':
                        msg = '找不到麦客风设备';
                        break;
                    default:
                        msg = '无法打开麦克风，异常信息:' + (error.code || error.name);
                        break;
                }
                if (config.funCancel) {
                    config.funCancel(msg);
                }
            });
    } else {
        if (config.funCancel) {
            config.funCancel('当前浏览器不支持录音功能');
        }
    }

    function log(str) {
        if (config.debug) {
            // console.log(str);
        }
    }
    function encode64(buffer) {
        var binary = "",
            bytes = new Uint8Array(buffer),
            len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        // console.log(binary)
        return window.btoa(binary);
    }
}
const defaultRecord = MP3Recorder({debug:true, initOK:()=>{console.log('init success')}, funCancel:(msg)=>{alert(msg);}});
export default defaultRecord;