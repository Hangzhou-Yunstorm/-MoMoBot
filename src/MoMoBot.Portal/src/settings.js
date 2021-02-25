const primaryColor = '#0078d7';
const warningColor = '#f2610c';
const dangerColor = '#a80000';

export default {
    warningColor,
    primaryColor,
    dangerColor,
    luisColors: [dangerColor, warningColor, primaryColor],
    siteKey: '6LetaZ4UAAAAAFO3y2qkLqAqxN3qbUePsFQpA_UI',
    serverUrl:  process.env.NODE_ENV === 'development' ?
        'http://10.0.1.46:4567' :
        'https://momoapi.yunstorm.com:4431'
}