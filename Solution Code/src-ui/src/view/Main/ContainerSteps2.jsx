import React from "react";
import {Layout,Button} from 'antd';
import ButtonsGroup from './ButtonsGroup';
import { GrLinkNext } from "react-icons/gr";


const ContainerSteps2 = () => {
    const { Content, Footer } = Layout;
    const component = "primary";

  return (
    <div className="p-4 h-full">
        <Layout>
            {/* <Header style={{ padding: 0 }} /> */}
            <ButtonsGroup
                component={component}
            />
            <Content style={{ margin: '24px 16px 0' }}>
                <div
                    style={{
                    padding: 24,
                    minHeight: 360
                    }}
                >
                    content
                </div>
            </Content>
            <Button
                icon={<GrLinkNext/>}
                className=""
                color="primary"
                variant="solid"
                >Next
            </Button>
        </Layout>
    </div>
  );
};

export default ContainerSteps2;
