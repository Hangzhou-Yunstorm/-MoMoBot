export default {
    //wsUrl:'http://52.184.99.37:4567',
    serverUrl: process.env.NODE_ENV === 'development' ? "http://10.0.1.46:4567" : 'https://momoapi.yunstorm.com:4431',
    ddUserDefaultValue: { "userid": "", "name": "", "department": ["0"] }
}