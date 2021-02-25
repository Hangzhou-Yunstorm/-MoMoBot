// https://umijs.org/config/
import os from 'os';
import slash from 'slash2';

const { NODE_ENV, APP_TYPE, TEST } = process.env;

const plugins = [
    [
        'umi-plugin-react',
        {
            antd: true,
            dva: {
                hmr: true,
            },
            locale: {
                enable: true, // default false
                default: 'zh-CN', // default zh-CN
                baseNavigator: true, // default true, when it is true, will use `navigator.language` overwrite default
            },
            dynamicImport: {
                loadingComponent: './components/PageLoading/index',
                webpackChunkName: true,
                level: 3,
            },
            ...(!TEST && os.platform() === 'darwin'
                ? {
                    dll: {
                        include: ['dva', 'dva/router', 'dva/saga', 'dva/fetch'],
                        exclude: ['@babel/runtime'],
                    },
                    hardSource: false,
                }
                : {}),
        },
    ],
];

const pageRoutes = [
    {
        path: '/account',
        component: '../layouts/AccountLayout',
        routes: [
            {
                path: '/account/login',
                component: './account/Login'
            },
            {
                path: '/account/logout',
                component: './account/Logout'
            }
        ]
    },
    {
        path: '/',
        component: '../layouts/BasicLayout',
        Routes: ['src/components/Authoirze'],
        routes: [{
            path: '/console',
            text: "控制台",
            hideInMenu: true,
            component: './Console',
        }, {
            path: '/',
            redirect: '/console',
        }, {
            path: '/luis',
            icon: 'robot',
            text: 'LUIS',
            routes: [
                { path: '/luis', redirect: '/luis/dashboard' },
                { path: '/luis/dashboard', component: './luis/Dashboard', text: 'LUIS 分析页' },
                { path: '/luis/knowledges', component: './luis/Knowledges', text: '知识库管理' },
                { path: '/luis/intents', component: './luis/Intents', text: '意图管理' },
                { path: '/luis/intents/:id', component: './luis/Intent' },
                { path: '/luis/unknown', component: './luis/Unknown', text: '未知回复' },
                { path: '/luis/flows', component: './luis/Flows/DialogFlows', text: '知识图谱' },
                { path: '/luis/flows/:id', component: './luis/Flows/DialogFlow' }
            ]
        }, {
            path: '/permission',
            icon: 'user',
            text: '权限管理',
            routes: [
                { path: '/permission', redirect: '/permission/departments' },
                { path: '/permission/departments', component: './permission/Departments', text: '组织架构' },
                { path: '/permission/modules', component: './permission/Modules', text: '模块管理' },
                { path: '/permission/users', component: './permission/Users', text: '用户管理' }
            ]
        }, {
            path: '/feedback',
            component: './Feedback',
            icon: 'coffee',
            text: '反馈'
        }, {
            path: '/im',
            component: '../layouts/IMLayout',
            icon: 'message',
            text: 'IM',
            hideChildrenInMenu: true,
            routes: [
                { path: '/im', redirect: "/im/chat" },
                { path: '/im/chat', component: './IM', text: 'IM' }
            ]
        }, {
            path: '/system',
            text: '系统管理',
            icon: 'setting',
            routes: [
                { path: '/system', redirect: '/system/settings' },
                { path: '/system/settings', text: '系统设置', component: './system/Settings' },
                { path: '/system/logs', text: '系统日志', component: './system/Logs' },
            ]
        }, {
            path: '/user/center',
            component: './user/Center'
        }, {
            path: '/test',
            component: './Test'
        }]
    }
]

export default {
    // add for transfer to umi
    plugins,
    define: {
        APP_TYPE: APP_TYPE || '',
    },
    treeShaking: true,
    targets: {
        ie: 11,
    },
    // 路由配置
    routes: pageRoutes,
    // Theme for antd
    // https://ant.design/docs/react/customize-theme-cn
    theme: {
        'primary-color': '#4EA9EC',
    },
    externals: {
        '@antv/data-set': 'DataSet',
        // bizcharts: 'BizCharts',
    },
    ignoreMomentLocale: true,
    lessLoaderOptions: {
        javascriptEnabled: true,
    },
    disableRedirectHoist: true,
    cssLoaderOptions: {
        modules: true,
        getLocalIdent: (context, localIdentName, localName) => {
            if (
                context.resourcePath.includes('node_modules') ||
                context.resourcePath.includes('ant.design.pro.less') ||
                context.resourcePath.includes('global.less')
            ) {
                return localName;
            }
            const match = context.resourcePath.match(/src(.*)/);
            if (match && match[1]) {
                const antdProPath = match[1].replace('.less', '');
                const arr = slash(antdProPath)
                    .split('/')
                    .map(a => a.replace(/([A-Z])/g, '-$1'))
                    .map(a => a.toLowerCase());
                return `antd-pro${arr.join('-')}-${localName}`.replace(/--/g, '-');
            }
            return localName;
        },
    },
    manifest: {
        basePath: '/',
    },
    // chainWebpack: webpackPlugin,
};