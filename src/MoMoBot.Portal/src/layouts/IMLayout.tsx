import * as React from 'react';
import ChatList from '../components/Chat/ChatList';
import { connect } from 'dva';
import IChatState from '../@types/IChatState';
import CustomerProfile from '../components/CustomerProfile';
import request from '../utils/request';
import settings from '../settings';
const styles = require('./IMLayout.less');

const RecordDetail = React.lazy(() => import('../components/RecordDetail/RecordDetail'));

class IMLayout extends React.Component<any, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            visible: false,
            childrenVisable: false,
            editable: false,
            submitting: false,
            record: {}
        }
    }

    componentDidMount() {
        const { dispatch } = this.props;
        const id = this.getId();
        console.log(id);

        dispatch({
            type: 'chat/fetchContacts',
            payload: id
        });
        // this.itemSelected(id);
    }

    componentWillUnmount() {
        const { dispatch } = this.props;
        dispatch({ type: 'chat/clear' })
    }

    getId = () => {
        let id = this.getIdFromUrl();
        if (id < 0 || isNaN(id)) {
            id = this.getFirstRecordId();
        }
        return id;
    }

    getFirstRecordId = () => {
        const { data } = this.props;
        if (data && data.length > 0) {
            return data[0].id;
        }
        return -1;
    }

    getIdFromUrl = () => {
        try {
            const { location: { query } } = this.props;
            const idStr = query['id'] || '-1';
            const id = Number.parseInt(idStr);
            return id;
        } catch{
            return -1;
        }
    }

    itemSelected = (recordId: number) => {
        const { activeRecord, dispatch } = this.props;
        const { id } = activeRecord || { id: -1 };
        if (recordId < 0 || id === recordId) {
            return;
        }
        dispatch({
            type: 'chat/changeRecordReadState',
            payload: recordId
        })
        dispatch({
            type: 'chat/changeCurrent',
            payload: {
                chatId: recordId
            }
        })
        dispatch({
            type: 'chat/fetchChatRecords',
            payload: {
                chatId: recordId
            }
        });
        dispatch({
            type: 'chat/fetchProfile',
            payload: recordId
        })
        dispatch({
            type: 'chat/fetchServiceRecords',
            payload: recordId
        })
    }

    fetchProfile = () => {
        const { activeRecord, dispatch } = this.props;
        const { id } = activeRecord || { id: -1 };
        dispatch({
            type: 'chat/fetchProfile',
            payload: id
        })
    }

    fetchServiceRecords = () => {
        const { activeRecord, dispatch } = this.props;
        const { id } = activeRecord || { id: -1 };
        dispatch({
            type: 'chat/fetchServiceRecords',
            payload: id
        })
    }

    recordClick = (record: any) => {
        console.log(record);
        this.setState({ visible: true, record, editable: !record.isDone })
    }

    render() {
        const { children, contacts, activeRecord, profile, serviceRecords, contactsLoading, location, profileLoading, recordsLoading } = this.props;
        const { id } = activeRecord || { id: -1 };
        const { visible, record, childrenVisable, editable, submitting } = this.state;
        const canReload = activeRecord && activeRecord.id;

        const formProps = {
            record,
            editable,
            submitting,
            onChatRecordOpen: () => { this.setState({ childrenVisable: true }); },
            onDelete: (id: number) => {
                this.setState({ submitting: true });
                request(`${settings.serverUrl}/api/servicerecord/remove/${id}`, {
                    method: 'delete'
                }).then(() => {
                    this.setState({ submitting: false, visible: false });
                    this.fetchServiceRecords();
                }).catch(() => {
                    this.setState({ submitting: false });
                })
            },
            onSubmit: (values: any) => {
                this.setState({ submitting: true });
                request(`${settings.serverUrl}/api/servicerecord/placeonfile`, {
                    method: 'POST',
                    body: {
                        ...values
                    }
                }).then(() => {
                    this.setState({ submitting: false, visible: false });
                    this.fetchServiceRecords();
                }).catch(() => {
                    this.setState({ submitting: false });
                })
            }
        }

        return (
            <div className={styles.container}>
                <div className={styles.listcontainer}>
                    <ChatList loading={contactsLoading}
                        activeId={id}
                        location={location}
                        data={contacts}
                        onItemSelected={this.itemSelected} />
                </div>
                <div className={styles.messageContainer}>
                    {children}
                </div>
                <div className={styles.profile}>
                    <CustomerProfile profile={{ customer: profile, canReload, loading: profileLoading, reload: this.fetchProfile }}
                        record={{ loading: recordsLoading, canReload, records: serviceRecords, reload: this.fetchServiceRecords, recordClick: this.recordClick }} />
                </div>

                <React.Suspense fallback={null}>
                    <RecordDetail visible={visible}
                        childrenVisable={childrenVisable}
                        formProps={formProps}
                        onChildrenDrawerClose={() => { this.setState({ childrenVisable: false }) }}
                        onClose={() => { this.setState({ visible: false }) }} />
                </React.Suspense>
            </div>
        )
    }
}

export default connect(({ chat, loading }: { chat: IChatState, loading: any }) => ({
    contacts: chat.contacts,
    activeRecord: chat.activeRecord,
    profile: chat.profile,
    records: chat.records,
    serviceRecords: chat.serviceRecords,
    contactsLoading: loading.effects['chat/fetchContacts'],
    profileLoading: loading.effects['chat/fetchProfile'],
    recordsLoading: loading.effects['chat/fetchServiceRecords']
}))(IMLayout);