import React, { Fragment } from 'react';
import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import { observer } from 'mobx-react-lite';
import HomePage from '../../features/home/HomePage';
import { Route, Switch, useLocation } from 'react-router-dom';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestErrors from '../../errors/TestError';
import { ToastContainer } from 'react-toastify';
import NotFound from '../../errors/NotFound';
import ServerError from '../../errors/ServerError';
import { useStore } from '../stores/store';
import { useEffect } from 'react';
import LoadingComponent from './LoadingComponent';
import ModalContainer from '../common/modals/ModalContainer';
import ProfilePage from '../../features/profiles/ProfilePage';
import PrivateRoute from './PrivateRoute';
import RegisterSuccess from '../../features/users/RegisterSuccess';
import ConfirmEmail from '../../features/users/ConfirmEmail';

function App() {

  const location = useLocation();
  const {commonStore, userStore} = useStore();
  
  useEffect(() => 
  {
    if(commonStore.token)
    {
      userStore.getUser().finally(() => commonStore.setAppLoaded());
    }
    else
    {
        userStore.getFacebookLoginStatus().then(() => commonStore.setAppLoaded());
    }
  }, [commonStore, userStore])

  if(!commonStore.appLoaded) return <LoadingComponent content='Loading app...' />

  return (
    <Fragment>
        <ToastContainer position='bottom-right' hideProgressBar />
        <ModalContainer />
        <Route exact path='/' component={HomePage} />
        <Route 
          path={'/(.+)'}
          render={() => (
            <>
              <NavBar />
              <Container style={{marginTop: '7em'}}>
                <Switch>
                  <PrivateRoute exact path='/activities' component={ActivityDashboard} />
                  <PrivateRoute path='/activities/:id' component={ActivityDetails} />
                  <PrivateRoute key={location.key} path={['/createActivity', '/manage/:id']} component={ActivityForm} />
                  <PrivateRoute path='/profiles/:username' component={ProfilePage} />
                  <PrivateRoute path='/errors' component={TestErrors} />
                  <Route path='/server-error' component={ServerError} />
                  <Route path='/account/registerSuccess' component={RegisterSuccess} />
                  <Route path='/account/verifyEmail' component={ConfirmEmail} />
                  <Route component={NotFound} />
                </Switch>
              </Container>
            </>
          )} 
        
        />
        
    </Fragment>
  );
}

export default observer(App);
