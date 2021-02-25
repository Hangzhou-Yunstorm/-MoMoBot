import * as pathToRegexp from 'path-to-regexp';

const TITLE = "小摩AI机器人";

export const matchParamsPath = (pathname: string, breadcrumbNameMap: string[]) => {
    if (breadcrumbNameMap && breadcrumbNameMap !== null) {
        const pathKey = Object.keys(breadcrumbNameMap).find(key => pathToRegexp(key).test(pathname));
        if (pathKey) {
            return breadcrumbNameMap[pathKey];
        }
    }
    return;
};

export const getTitle = (pathname: string, breadcrumbNameMap: string[]) => {
    const currRouterData = matchParamsPath(pathname, breadcrumbNameMap);
    if (!currRouterData) {
        return;
    }
    return currRouterData.text
}

export const getPageTitle = (pathname: string, breadcrumbNameMap: string[]) => {

    const title = getTitle(pathname, breadcrumbNameMap);
    if (title) {
        return `${title} - ${TITLE}`;
    }
    return TITLE;
};