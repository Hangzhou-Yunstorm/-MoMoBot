import PageLayout from './PageLayout';
import { getTitle } from '../../utils/getPageTitle';

export default (props) => {
    const { location: { pathname }, breadcrumbNameMap } = props;
    const title = getTitle(pathname, breadcrumbNameMap)
    return <PageLayout {...props} title={title} />
}